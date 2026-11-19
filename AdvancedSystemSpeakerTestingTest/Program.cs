using NeoBleeper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace AdvancedSystemSpeakerProbe;

internal static class Program
{
    private const int DivisorMax = 65535;
    private const int DivisorMid = 16384;
    private const double PitClock = 1_193_182.0;
    private const double ExpectedHz = PitClock / DivisorMax;

    private static PcSpeakerDevice _device = null!;

    private static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Title = "Advanced System Speaker Probe - PawnIO";
        PrintLogo();
        PrintBanner();

        if (!OperatingSystem.IsWindows() || !Environment.Is64BitProcess)
        {
            PrintError("This program requires x64 Windows and an x64 PawnIO installation.");
            return 1;
        }

        if (ProgramPrecheck.IsPawnIOInstalled() is false)
        {
            PrintError("PawnIO driver not installed or not active.");
            Console.WriteLine("  Install the official PawnIO driver and ensure it is running.");
            return 1;
        }

        if (ProgramPrecheck.IsRanAsAdmin() is false)
        {
            PrintError("This program must be run as Administrator.");
            Console.WriteLine("  Right-click the executable and select 'Run as administrator'.");
            return 1;
        }
        // PcSpeaker.p is SOURCE CODE. pawnio_load requires a compiled and signed module blob.
        string modulePath = args.Length > 0
            ? Path.GetFullPath(args[0])
            : Path.Combine(AppContext.BaseDirectory, "PcSpeaker.bin");

        if (!File.Exists(modulePath))
        {
            PrintError($"Signed PcSpeaker module not found: {modulePath}");
            Console.WriteLine("  Download PcSpeaker.bin from an official signed PawnIO.Modules release.");
            Console.WriteLine("  Place it beside the executable, or provide its full path as argument 1.");
            Console.WriteLine("  The .p source code cannot be passed directly to pawnio_load.");
            return 2;
        }

        try
        {
            using var pawnio = new PawnIOWrapper(File.ReadAllBytes(modulePath));
            _device = new PcSpeakerDevice(pawnio);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  PawnIO version : 0x{PawnIOWrapper.Version():X8}");
            Console.WriteLine($"  Module         : {modulePath}");
            Console.WriteLine("  I/O interface  : PcSpeaker.p / ioctl_batch (no InpOutx64)");
            Console.WriteLine("  Pulse          : 2 x 50us kernel stalls, same batch as disable");
            Console.WriteLine("  Detection      : PIT/PPI functionality only; NOT proof of transducer");
            Console.ResetColor();
            Console.WriteLine();

            TestSuite suite = new();
            suite.RunAll();
            suite.PrintSummary();
            Console.WriteLine();
            RunVerificationPhase();
            Console.WriteLine();
            Console.Write("  Press any key to exit...");
            if (!Console.IsInputRedirected) Console.ReadKey(intercept: true);
            Console.WriteLine();
            return suite.FailureCount == 0 ? 0 : 3;
        }
        catch (Exception ex) when (ex is DllNotFoundException or BadImageFormatException
                                   or COMException or UnauthorizedAccessException
                                   or InvalidOperationException or IOException)
        {
            PrintError($"PawnIO initialization / module loading failed: {ex.Message}");
            Console.WriteLine("  Check that the official PawnIO driver is installed and active,");
            Console.WriteLine("  the process is x64, and PcSpeaker.bin is a signed/accepted build.");
            Console.WriteLine("  A source-only PcSpeaker.p, even if locally compiled, is not a signed module.");
            return 4;
        }
    }

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  ERROR: {message}");
        Console.ResetColor();
    }

    private static void DelayUs(long microseconds)
    {
        if (microseconds <= 0) return;
        long deadline = Stopwatch.GetTimestamp() +
            (long)Math.Max(1, microseconds * (Stopwatch.Frequency / 1_000_000.0));
        while (Stopwatch.GetTimestamp() < deadline)
            Thread.SpinWait(16);
    }

    private static string DecodePort61(byte v) =>
        $"0x{v:X2} [{Convert.ToString(v, 2).PadLeft(8, '0')}] " +
        $"gate={(v & 1)} data={((v >> 1) & 1)} PIT_OUT={((v >> 5) & 1)}";

    private static TestResult WithPortRestore(Func<byte, TestResult> action)
    {
        byte previous = _device.ReadPort61();
        try { return action(previous); }
        finally { _device.RestoreControl(previous); }
    }

    private enum Verdict { Pass, Warn, Fail }

    private sealed record TestResult(Verdict Verdict, string Details)
    {
        public static TestResult Pass(string text) => new(Verdict.Pass, text);
        public static TestResult Warn(string text) => new(Verdict.Warn, text);
        public static TestResult Fail(string text) => new(Verdict.Fail, text);
    }

    private sealed class TestSuite
    {
        private readonly List<(string Name, TestResult Result)> _results = new();
        public int FailureCount => _results.Count(x => x.Result.Verdict == Verdict.Fail);

        public void RunAll()
        {
            Execute("T01  Port 0x61 Readability", T01_Port61Read);
            Execute("T02  Port 0x61 Bit-0 Write / Read-Back", T02_Bit0RoundTrip);
            Execute("T03  PIT Ch2 Square-Wave Program", T03_PitProgram);
            Execute("T04  PIT Count Latch — Timer Running", T04_PitCountMoving);
            Execute("T05  PIT Read-Back Status Decode", T05_PitReadBackStatus);
            Execute("T06  Bit-5 Oscillation [Gate ON / Spk OFF]", T06_Bit5OscGateOnly);
            Execute("T07  Bit-5 Stability Control [Gate OFF]", T07_Bit5StaticGateOff);
            Execute("T08  Sub-Audio Probe [18 Hz / ~100 µs]", T08_SubAudioProbe);
            Execute("T09  PIT Count In-Range at Two Divisors", T09_TwoDivisors);
            Execute("T10  Bit-5 Frequency Estimation", T10_FrequencyEstimation);
        }

        private void Execute(string name, Func<TestResult> fn)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"  ► {name,-50}");
            Console.ResetColor();
            TestResult result;
            try { result = fn(); }
            catch (Exception ex) { result = TestResult.Fail($"Exception: {ex.GetType().Name}: {ex.Message}"); }

            Console.ForegroundColor = result.Verdict switch
            {
                Verdict.Pass => ConsoleColor.Green,
                Verdict.Warn => ConsoleColor.Yellow,
                _ => ConsoleColor.Red
            };
            Console.WriteLine($"[{result.Verdict.ToString().ToUpperInvariant()}]");
            Console.ResetColor();
            foreach (string line in result.Details.Split('\n'))
                if (!string.IsNullOrWhiteSpace(line))
                    Console.WriteLine($"               {line.Trim()}");
            _results.Add((name, result));
        }

        public void PrintSummary()
        {
            int pass = _results.Count(x => x.Result.Verdict == Verdict.Pass);
            int warn = _results.Count(x => x.Result.Verdict == Verdict.Warn);
            int fail = FailureCount;
            Console.WriteLine("  ═══════════════════════════════════════════════════════");
            Console.WriteLine($"  RESULTS  Pass: {pass}  Warn: {warn}  Fail: {fail}  Total: {_results.Count}");
            Console.WriteLine("  ═══════════════════════════════════════════════════════");

            bool t06 = _results.Any(x => x.Name.StartsWith("T06") && x.Result.Verdict == Verdict.Pass);
            bool t10 = _results.Any(x => x.Name.StartsWith("T10") && x.Result.Verdict == Verdict.Pass);
            Console.WriteLine();
            if (t06 && t10)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  Overall: PIT CH2 / PPI FEEDBACK FUNCTIONING");
                Console.ResetColor();
                Console.WriteLine("  Observed channel-2 output transitions and a plausible PIT frequency.");
            }
            else if (t06 || t10)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Overall: PARTIAL PIT / PPI FEEDBACK");
                Console.ResetColor();
                Console.WriteLine("  Some feedback observed, but tests disagree or sampling is inconsistent.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("  Overall: PIT / PPI FEEDBACK NOT CONFIRMED");
                Console.ResetColor();
                Console.WriteLine("  No reliable channel-2 transitions detected; hardware may be unsupported or virtualized.");
            }
            Console.WriteLine("  IMPORTANT: PIT counter or bit-5 activity does NOT establish a connected physical speaker.");
            Console.WriteLine("  An emulated PIT and an unpopulated speaker header can produce identical readings.");

            foreach (var item in _results.Where(x => x.Result.Verdict != Verdict.Pass))
                Console.WriteLine($"  {item.Result.Verdict}: {item.Name} — {item.Result.Details.Split('\n')[0]}");
            Console.WriteLine("  Check: official PawnIO driver, signed PcSpeaker.bin, real (non-VM) hardware,");
            Console.WriteLine("         UEFI settings, and interference from other PIT/speaker users.");
        }
    }

    private static TestResult T01_Port61Read()
    {
        byte value = _device.ReadPort61();
        return TestResult.Pass(DecodePort61(value) + " — PPI read IOCTL completed");
    }

    private static TestResult T02_Bit0RoundTrip() => WithPortRestore(original =>
    {
        // Mask 0x03: preserve every other bit, clear data while toggling gate.
        _device.WriteControl(0);
        DelayUs(400);
        byte low = _device.ReadPort61();
        _device.WriteControl(1);
        DelayUs(400);
        byte high = _device.ReadPort61();
        bool okLow = (low & 3) == 0, okHigh = (high & 3) == 1;
        string detail = $"Gate OFF: {DecodePort61(low)} [{(okLow ? "OK" : "MISMATCH")}]\n" +
                        $"Gate ON : {DecodePort61(high)} [{(okHigh ? "OK" : "MISMATCH")}]";
        return okLow && okHigh ? TestResult.Pass(detail) : TestResult.Fail(detail);
    });

    private static TestResult T03_PitProgram() => WithPortRestore(original =>
    {
        _device.WriteControl(0); // Avoid sounding an already-enabled speaker while programming.
        _device.ProgramPit(DivisorMax);
        return TestResult.Pass($"Channel 2 mode 3 accepted; divisor {DivisorMax} (~{ExpectedHz:F2} Hz). " +
                               "An accepted command does not confirm actual timer operation.");
    });

    private static TestResult T04_PitCountMoving() => WithPortRestore(original =>
    {
        _device.WriteControl(0);
        _device.ProgramPit(DivisorMax);
        _device.WriteControl(1); // gate ON, speaker-data OFF
        DelayUs(1_000);
        int a = _device.LatchCount();
        DelayUs(3_000);
        int b = _device.LatchCount();
        string detail = $"Sample A: {a} (0x{a:X4})\nSample B: {b} (0x{b:X4})";
        return a != b ? TestResult.Pass(detail) : TestResult.Warn(detail + "\nCounts did not change.");
    });

    private static TestResult T05_PitReadBackStatus() => WithPortRestore(original =>
    {
        _device.WriteControl(0);
        _device.ProgramPit(DivisorMax);
        _device.WriteControl(1);
        DelayUs(1_000);
        var (status, count) = _device.ReadbackStatusAndCount();
        bool outputHigh = (status & 0x80) != 0;
        bool nullCount = (status & 0x40) != 0;
        int access = (status >> 4) & 3;
        int mode = (status >> 1) & 7;
        bool bcd = (status & 1) != 0;
        bool ok = access == 3 && (mode == 3 || mode == 7) && !bcd && !nullCount;
        string detail = $"Status 0x{status:X2}: OUT={outputHigh}, null={nullCount}, " +
                        $"access={access}, mode={mode}, BCD={bcd}\nCount: {count} (0x{count:X4})";
        return ok ? TestResult.Pass(detail) : TestResult.Fail(detail);
    });

    private static int CountBit5Transitions(int windowMilliseconds, int sampleDelayMicroseconds,
        out long elapsedMilliseconds)
    {
        int transitions = 0;
        bool? prior = null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        while (stopwatch.ElapsedMilliseconds < windowMilliseconds)
        {
            bool current = (_device.ReadPort61() & 0x20) != 0;
            if (prior.HasValue && prior.Value != current) transitions++;
            prior = current;
            DelayUs(sampleDelayMicroseconds);
        }
        stopwatch.Stop();
        elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        return transitions;
    }

    private static TestResult T06_Bit5OscGateOnly() => WithPortRestore(original =>
    {
        _device.WriteControl(0);
        _device.ProgramPit(DivisorMax);
        _device.WriteControl(1);
        DelayUs(2_000);
        int transitions = CountBit5Transitions(700, 3_000, out long elapsed);
        string detail = $"{transitions} bit-5 transitions in {elapsed} ms; gate ON / speaker-data OFF.";
        return transitions >= 4 ? TestResult.Pass(detail) : TestResult.Fail(detail);
    });

    private static TestResult T07_Bit5StaticGateOff() => WithPortRestore(original =>
    {
        _device.WriteControl(0);
        DelayUs(800);
        int transitions = CountBit5Transitions(300, 100, out long elapsed);
        string detail = $"{transitions} bit-5 transitions in {elapsed} ms with gate OFF.";
        return transitions == 0 ? TestResult.Pass(detail)
            : TestResult.Warn(detail + " Hypervisor or platform may ignore PIT gate.");
    });

    private static TestResult T08_SubAudioProbe() => WithPortRestore(original =>
    {
        _device.Pulse100Microseconds(DivisorMax);
        byte after = _device.ReadPort61();
        bool silent = (after & 3) == 0;
        string detail = $"PIT ~{ExpectedHz:F2} Hz; speaker data enabled for nominal 100us " +
                        "(2 x 50us kernel stalls) then disabled in the SAME batch.\n" +
                        $"Read-back after pulse: {DecodePort61(after)}. " +
                        "A brief electrical click may still be possible.";
        return silent ? TestResult.Pass(detail) : TestResult.Fail(detail);
    });

    private static TestResult T09_TwoDivisors() => WithPortRestore(original =>
    {
        _device.WriteControl(0);
        _device.ProgramPit(DivisorMax);
        _device.WriteControl(1);
        DelayUs(1_500);
        int a = _device.LatchCount();
        _device.ProgramPit(DivisorMid);
        DelayUs(1_500);
        int b = _device.LatchCount();
        bool ok = a >= 0 && a <= DivisorMax && b >= 0 && b <= DivisorMid;
        string detail = $"Divisor {DivisorMax}: count {a} (0..{DivisorMax})\n" +
                        $"Divisor {DivisorMid}: count {b} (0..{DivisorMid})";
        return ok ? TestResult.Pass(detail) : TestResult.Fail(detail);
    });

    private static TestResult T10_FrequencyEstimation() => WithPortRestore(original =>
    {
        _device.WriteControl(0);
        _device.ProgramPit(DivisorMax);
        _device.WriteControl(1);
        DelayUs(2_000);
        int transitions = CountBit5Transitions(1_500, 2_000, out long elapsed);
        double hz = elapsed > 0 ? transitions / 2.0 / (elapsed / 1_000.0) : 0;
        double error = Math.Abs(hz - ExpectedHz);
        string detail = $"{transitions} transitions in {elapsed} ms; measured {hz:F2} Hz, " +
                        $"expected {ExpectedHz:F2} Hz, error {error:F2} Hz.";
        return transitions >= 2 && error <= 8 ? TestResult.Pass(detail)
             : transitions >= 2 ? TestResult.Warn(detail + " Approximate timing may be scheduler-limited.")
             : TestResult.Fail(detail);
    });

    private static void RunVerificationPhase()
    {
        Console.WriteLine("  ═══════════════════════════════════════════════════════════════════════");
        Console.WriteLine("  REVERSE CHECKER: PPI CONTROL-PATH READBACK (NOT SPEAKER DETECTION)");
        Console.WriteLine("  ═══════════════════════════════════════════════════════════════════════");
        try
        {
            TestResult result = WithPortRestore(original =>
            {
                // No port 0x80 write or realtime process priority: neither is supported
                // nor does I/O call latency measure speaker-coil impedance.
                var (off, on, after) = _device.VerifySpeakerDataControl();
                bool okOff = (off & 3) == 1;
                bool okOn = (on & 3) == 3;
                bool okAfter = (after & 3) == 1;
                string detail = $"Bit 1 OFF : {DecodePort61(off)}\n" +
                                $"Bit 1 ON  : {DecodePort61(on)}\n" +
                                $"Restored OFF within batch: {DecodePort61(after)}";
                return okOff && okOn && okAfter ? TestResult.Pass(detail) : TestResult.Warn(detail);
            });
            Console.WriteLine($"  PPI CONTROL READBACK: {result.Verdict}");
            Console.WriteLine(result.Details);
            Console.WriteLine("  VERDICT: PPI latch behavior only. No physical speaker presence/absence conclusion.");
        }
        catch (Exception ex)
        {
            PrintError($"Verification phase failed: {ex.Message}");
        }
    }

    private static void PrintLogo()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ╔═╗╔╦╗╦  ╦╔═╗╔╗╔╔═╗╔═╗╔╦╗  ╔═╗╔═╗╔═╗╔═╗╦╔═╔═╗╦═╗");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ╠═╣ ║║╚╗╔╝╠═╣║║║║  ║╣  ║║  ╚═╗╠═╝║╣ ╠═╣╠╩╗║╣ ╠╦╝");
        Console.WriteLine("  ╩ ╩═╩╝ ╚╝ ╩ ╩╝╚╝╚═╝╚═╝═╩╝  ╚═╝╩  ╚═╝╩ ╩╩ ╩╚═╝╩╚═");
        Console.ResetColor();
        Console.WriteLine("        S I L E N T  E D I T I O N  —  P a w n I O");
    }

    private static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("  ═══════════════════════════════════════════════════════════════");
        Console.WriteLine("    ADVANCED SYSTEM SPEAKER PROBE — 10 TESTS — PawnIO");
        Console.WriteLine("  ═══════════════════════════════════════════════════════════════");
        Console.ResetColor();
    }
}
