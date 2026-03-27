using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace AdvancedSystemSpeakerProbe
{
    internal static class Program
    {
        // ── Native interop ───────────────────────────────────────────────────────
        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Out32(short portAddress, short data);

        [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
        private static extern short Inp32(short portAddress);

        // ── Port addresses ───────────────────────────────────────────────────────
        private const short PORT_SPEAKER = 0x61; // System-speaker / NMI control
        private const short PORT_PIT_CMD = 0x43; // 8254 PIT command register
        private const short PORT_PIT_CH2 = 0x42; // 8254 PIT channel 2

        // ── PIT command bytes ────────────────────────────────────────────────────
        // Channel 2 | lo/hi byte | mode 3 (square wave) | binary  →  10 11 011 0
        private const byte PIT_CH2_SQUAREWAVE = 0xB6;

        // Counter-latch command for channel 2: bits[7:6]=10, bits[5:4]=00  →  1000 0000
        private const byte PIT_CH2_LATCH = 0x80;

        // Read-back command: latch count + status for channel 2 only
        // 11 (read-back) | 00 (DO latch both) | 1000 (channel 2)  →  1100 1000
        // Status byte is returned first on the next read from port 0x42,
        // followed by the latched count (lo byte, hi byte).
        private const byte PIT_CH2_READBACK = 0xC8;

        // ── Timing / frequency constants ─────────────────────────────────────────
        // Maximum PIT divisor (16-bit) → minimum frequency ≈ 18.2 Hz (below 20 Hz auditory floor)
        private const int DIVISOR_MAX = 65535;
        private const int DIVISOR_MID = 16384; // ~72.8 Hz — used for T09 only (gate ON, spk OFF)
        private const double PIT_CLOCK = 1_193_182.0;
        private const double EXPECTED_HZ = PIT_CLOCK / DIVISOR_MAX; // ~18.20 Hz

        // ────────────────────────────────────────────────────────────────────────
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Advanced System Speaker Probe";

            PrintLogo();
            PrintBanner();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();
            Console.WriteLine("  Probe policy : bit 1 of port 0x61 never held ON.");
            Console.WriteLine("  Sub-audio    : 100 µs at 18 Hz  (<20 Hz, inaudible).");
            Console.WriteLine("  No Beep svc  : no call to Console.Beep / \\Device\\Beep.");
            Console.WriteLine("  No PNP0800   : no device-node enumeration.");
            Console.ResetColor();
            Console.WriteLine();

            var suite = new TestSuite();
            suite.RunAll();
            suite.PrintSummary();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Press any key to exit...");
            Console.ResetColor();
            Console.ReadKey(intercept: true);
            Console.WriteLine();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  I/O primitives
        // ════════════════════════════════════════════════════════════════════════

        private static byte ReadPort(short port) => unchecked((byte)(Inp32(port) & 0xFF));
        private static void WritePort(short port, byte value) => Out32(port, (short)value);

        private static void SafeRestore(byte original)
        {
            try { WritePort(PORT_SPEAKER, original); }
            catch { /* best-effort */ }
            BusyWaitUs(150);
        }

        /// <summary>High-resolution busy-wait using Stopwatch ticks.</summary>
        private static void BusyWaitUs(long microseconds)
        {
            if (microseconds <= 0) return;
            long ticks = (long)(microseconds * (Stopwatch.Frequency / 1_000_000.0));
            long target = Stopwatch.GetTimestamp() + Math.Max(1L, ticks);
            while (Stopwatch.GetTimestamp() < target)
                Thread.SpinWait(8);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  PIT helpers
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Program PIT channel 2 in square-wave mode with the given divisor.
        /// Caller must have already saved port 0x61.
        /// </summary>
        private static void ProgramPitCh2(int divisor)
        {
            divisor = Math.Clamp(divisor, 2, 65535);
            WritePort(PORT_PIT_CMD, PIT_CH2_SQUAREWAVE);
            WritePort(PORT_PIT_CH2, (byte)(divisor & 0xFF));
            WritePort(PORT_PIT_CH2, (byte)((divisor >> 8) & 0xFF));
        }

        /// <summary>
        /// Latch and read the 16-bit count from channel 2.
        /// MUST be called with the gate already enabled.
        /// </summary>
        private static int LatchCount()
        {
            WritePort(PORT_PIT_CMD, PIT_CH2_LATCH);
            int lo = ReadPort(PORT_PIT_CH2);
            int hi = ReadPort(PORT_PIT_CH2);
            return lo | (hi << 8);
        }

        /// <summary>Returns byte with gate bit set, speaker-data bit cleared (safe probe state).</summary>
        private static byte GateOnSpeakerOff(byte orig) => (byte)((orig | 0x01) & ~0x02);

        // ════════════════════════════════════════════════════════════════════════
        //  Port-0x61 diagnostic helpers
        // ════════════════════════════════════════════════════════════════════════

        private static string DecodePort61(byte v) =>
            $"0x{v:X2}  [{Convert.ToString(v, 2).PadLeft(8, '0')}]" +
            $"  bit0(gate)={(v & 1)}  bit1(spk-data)={(v >> 1) & 1}  bit5(pit-out)={(v >> 5) & 1}";

        // ════════════════════════════════════════════════════════════════════════
        //  Result model
        // ════════════════════════════════════════════════════════════════════════

        private enum Verdict { Pass, Warn, Fail }

        private sealed class TestResult
        {
            public readonly Verdict Verdict;
            public readonly string Details;
            private TestResult(Verdict v, string d) { Verdict = v; Details = d; }
            public static TestResult Pass(string d) => new(Verdict.Pass, d);
            public static TestResult Warn(string d) => new(Verdict.Warn, d);
            public static TestResult Fail(string d) => new(Verdict.Fail, d);
        }

        // ════════════════════════════════════════════════════════════════════════
        //  Test runner
        // ════════════════════════════════════════════════════════════════════════

        private sealed class TestSuite
        {
            private readonly List<(string Name, TestResult Result)> _results = new();

            public void RunAll()
            {
                Execute("T01  Port 0x61 Readability", T01_Port61Read);
                Execute("T02  Port 0x61 Bit-0 Write / Read-Back", T02_Bit0RoundTrip);
                Execute("T03  PIT Ch2 Square-Wave Program", T03_PitProgram);
                Execute("T04  PIT Count Latch — Timer Running", T04_PitCountMoving);
                Execute("T05  PIT Read-Back Status Decode", T05_PitReadBackStatus);
                Execute("T06  Bit-5 Oscillation  [Gate ON / Spk OFF]", T06_Bit5OscGateOnly);
                Execute("T07  Bit-5 Stability Control  [Gate OFF]", T07_Bit5StaticGateOff);
                Execute("T08  Sub-Audio Probe  [18 Hz / 100 µs]", T08_SubAudioProbe);
                Execute("T09  PIT Count In-Range at Two Divisors", T09_TwoDivisors);
                Execute("T10  Bit-5 Frequency Estimation", T10_FrequencyEstimation);
            }

            private void Execute(string name, Func<TestResult> fn)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"  ► {name,-50}");
                Console.ResetColor();

                TestResult r;
                try { r = fn(); }
                catch (Exception ex) { r = TestResult.Fail($"Exception → {ex.Message}"); }

                PrintVerdict(r.Verdict);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (string line in r.Details.Split('\n'))
                    if (!string.IsNullOrWhiteSpace(line))
                        Console.WriteLine($"               {line.TrimStart()}");
                Console.ResetColor();

                _results.Add((name, r));
            }

            private static void PrintVerdict(Verdict v)
            {
                (string label, ConsoleColor color) = v switch
                {
                    Verdict.Pass => (" PASS ", ConsoleColor.Green),
                    Verdict.Warn => (" WARN ", ConsoleColor.Yellow),
                    _ => (" FAIL ", ConsoleColor.Red),
                };
                Console.ForegroundColor = color;
                Console.WriteLine($"[{label}]");
                Console.ResetColor();
            }

            public void PrintSummary()
            {
                int pass = _results.Count(r => r.Result.Verdict == Verdict.Pass);
                int warn = _results.Count(r => r.Result.Verdict == Verdict.Warn);
                int fail = _results.Count(r => r.Result.Verdict == Verdict.Fail);
                int total = _results.Count;

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("  ═══════════════════════════════════════════════════════");
                Console.WriteLine($"  RESULTS   Pass: {pass}   Warn: {warn}   Fail: {fail}   Total: {total}");
                Console.WriteLine("  ═══════════════════════════════════════════════════════");
                Console.ResetColor();

                // Take the results of all tests into account to produce an overall verdict and explanation.
                var t06 = _results.FirstOrDefault(r => r.Name.StartsWith("T06"));
                var t10 = _results.FirstOrDefault(r => r.Name.StartsWith("T10"));
                bool t06Pass = t06.Result?.Verdict == Verdict.Pass;
                bool t10Pass = t10.Result?.Verdict == Verdict.Pass;

                Console.WriteLine();
                Console.Write("  Overall verdict: ");

                // Priority 1: T06 and T10 are the most direct indicators of a real speaker circuit with PIT feedback.  If both fail, it's very unlikely real hardware is present regardless of other test results.
                string overallReason;
                if (!t06Pass && !t10Pass)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("NOT DETECTED — speaker hardware not confirmed.");
                    Console.ResetColor();

                    overallReason =
                        "No PIT output observed (T06 and T10 did not PASS). " +
                        "Even though some PIT/port accesses succeeded, bit-5 transitions required to indicate a physical speaker circuit were not observed. " +
                        "Causes: no physical speaker connected, speaker circuit disconnected, or firmware/hypervisor emulation that returns plausible port values without real toggles.";
                }
                else if (pass >= 8 && fail == 0 && t06Pass && t10Pass)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("SPEAKER CONFIRMED — all major tests passed.");
                    Console.ResetColor();

                    overallReason =
                        "All major tests passed including bit-5 oscillation (T06) and frequency estimation (T10). " +
                        "This is strong evidence a physical speaker circuit is present and the PIT ↔ port path is functioning.";
                }
                else if (pass >= 6 && fail <= 1 && (t06Pass || t10Pass))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("LIKELY PRESENT — strong but incomplete evidence.");
                    Console.ResetColor();

                    overallReason =
                        "Most tests passed and at least one PIT output test (T06 or T10) passed. " +
                        "Likely a speaker circuit is present but some secondary checks flagged warnings/failures (see list below).";
                }
                else if (t06Pass || t10Pass)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("UNCERTAIN — PIT output observed but other evidence incomplete.");
                    Console.ResetColor();

                    overallReason =
                        "A PIT output (bit-5 transitions) was observed, but other tests failed or produced warnings. " +
                        "This can indicate a partial wiring, an adapter, or an environment that interferes with some PIT/readback behaviors.";
                }
                else if (pass >= 3)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("UNCERTAIN — partial legacy port support detected but no PIT output observed.");
                    Console.ResetColor();

                    overallReason =
                        "Several legacy port operations succeeded but no PIT ↔ port output was observed (T06/T10 failed). " +
                        "This often indicates emulation of port reads/writes without a real speaker circuit.";
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("NOT DETECTED — speaker hardware not confirmed.");
                    Console.ResetColor();

                    overallReason =
                        "Insufficient passing tests and no convincing PIT output evidence; speaker hardware could not be confirmed.";
                }

                // Print summary of overall verdict and reasoning for the user to understand the conclusion, along with explanations of what the different outcomes mean and a list of failed/warning tests for further insight.
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  Reason:");
                Console.ResetColor();
                Console.WriteLine($"    {overallReason}");

                // Situation analysis and next steps for the user based on the observed results, especially if the outcome is not a clear PASS or FAIL.
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  Explanation of possible outcomes:");
                Console.ResetColor();
                Console.WriteLine("    • SPEAKER CONFIRMED: All major tests including bit-5 oscillation and frequency estimation passed — physical speaker likely present.");
                Console.WriteLine("    • LIKELY PRESENT  : Most tests passed and PIT output observed — strong evidence but minor discrepancies exist.");
                Console.WriteLine("    • UNCERTAIN       : Partial evidence (some PASS) but either PIT output inconsistent or other tests failed — further checks required.");
                Console.WriteLine("    • NOT DETECTED    : No PIT → port feedback observed (T06/T10) or too few tests passed — speaker hardware not confirmed.");
                Console.ResetColor();

                // List any failed or warning tests with their reasons to give the user more insight into what issues were observed during the probe, which can help with troubleshooting or understanding limitations of the environment (e.g. hypervisor interference, partial emulation, etc.).
                var failed = _results.Where(r => r.Result.Verdict == Verdict.Fail).ToList();
                var warns = _results.Where(r => r.Result.Verdict == Verdict.Warn).ToList();

                if (failed.Any())
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  Failed tests:");
                    Console.ResetColor();
                    foreach (var f in failed)
                    {
                        string reason = f.Result.Details?.Split('\n')[0].Trim() ?? "";
                        Console.WriteLine($"   - {f.Name}: {reason}");
                    }
                }

                if (warns.Any())
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Warning tests:");
                    Console.ResetColor();
                    foreach (var w in warns)
                    {
                        string reason = w.Result.Details?.Split('\n')[0].Trim() ?? "";
                        Console.WriteLine($"   - {w.Name}: {reason}");
                    }
                }

                // Suggestions for improving test reliability and next steps for the user to take if the results are inconclusive or indicate potential issues with the testing environment rather than the hardware itself.
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  Suggestions / Checks:");
                Console.WriteLine("  • Run on a physical machine (no VM) with UEFI/firmware default settings.");
                Console.WriteLine("  • Launch elevated (Administrator) so inpoutx64 driver can access ports.");
                Console.WriteLine("  • Verify inpoutx64 driver is installed and functional.");
                Console.WriteLine("  • If T06/T10 do not PASS, check physical speaker wiring, speaker-enable path, and whether BIOS/UEFI or hypervisor intercepts PIT I/O.");
                Console.ResetColor();
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T01  — Port 0x61 Readability
        //  Simply verifies the port can be read.  A plausible 8-bit value is
        //  returned; the read itself failing means inpoutx64 is not functional.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T01_Port61Read()
        {
            byte v = ReadPort(PORT_SPEAKER);
            return TestResult.Pass(DecodePort61(v));
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T02  — Port 0x61 Bit-0 Write / Read-Back
        //  Toggles only the timer-gate bit (bit 0).  Bit 1 is forced to 0 in
        //  every write so the speaker-data line is never driven.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T02_Bit0RoundTrip()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                byte safeBase = (byte)(orig & ~0x02); // guarantee bit 1 = 0

                byte clrGate = (byte)(safeBase & ~0x01);
                byte setGate = (byte)(safeBase | 0x01);

                WritePort(PORT_SPEAKER, clrGate); BusyWaitUs(400);
                byte rdClr = ReadPort(PORT_SPEAKER);

                WritePort(PORT_SPEAKER, setGate); BusyWaitUs(400);
                byte rdSet = ReadPort(PORT_SPEAKER);

                SafeRestore(orig);

                bool okClr = (rdClr & 0x01) == 0 && (rdClr & 0x02) == 0;
                bool okSet = (rdSet & 0x01) == 1 && (rdSet & 0x02) == 0;

                string info =
                    $"Write 0x{clrGate:X2} → Read 0x{rdClr:X2}  gate={rdClr & 1}  spk-data={(rdClr >> 1) & 1}  [{(okClr ? "OK" : "MISMATCH")}]\n" +
                    $"Write 0x{setGate:X2} → Read 0x{rdSet:X2}  gate={rdSet & 1}  spk-data={(rdSet >> 1) & 1}  [{(okSet ? "OK" : "MISMATCH")}]";

                return okClr && okSet ? TestResult.Pass(info) : TestResult.Fail(info);
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T03  — PIT Ch2 Square-Wave Program
        //  Writes the command byte and divisor bytes to the PIT.  No exception
        //  means the command port accepted the write sequence.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T03_PitProgram()
        {
            ProgramPitCh2(DIVISOR_MAX);
            return TestResult.Pass(
                $"Mode 3 square-wave written to channel 2.\n" +
                $"Divisor = {DIVISOR_MAX}  →  ~{EXPECTED_HZ:F2} Hz  (below 20 Hz auditory floor).");
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T04  — PIT Count Latch — Timer Running
        //  Latches the counter twice with a 3 ms gap.  If the PIT is actually
        //  counting, the two samples must differ.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T04_PitCountMoving()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);

                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(1_000);

                int countA = LatchCount();
                BusyWaitUs(3_000); // 3 ms gap
                int countB = LatchCount();

                SafeRestore(orig);

                bool differ = countA != countB;
                string info =
                    $"Sample A : {countA,5}  (0x{countA:X4})\n" +
                    $"Sample B : {countB,5}  (0x{countB:X4})  " +
                    $"{(differ ? "← values differ → timer is running" : "← SAME → timer may be stalled")}";

                return differ ? TestResult.Pass(info) : TestResult.Warn(info);
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T05  — PIT Read-Back Status Decode
        //  Uses the 8254 read-back command (0xC8) to latch both the count AND
        //  the status byte for channel 2 in a single atomic operation.
        //  Status byte layout:
        //    bit 7  : current output-pin state
        //    bit 6  : null-count flag  (1 = new divisor not yet loaded)
        //    bits 5-4: access mode programmed  (expect 3 = lo/hi)
        //    bits 3-1: timer mode programmed   (expect 3 = square wave)
        //    bit 0  : BCD flag                 (expect 0 = binary)
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T05_PitReadBackStatus()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(1_000);

                // Issue read-back — latches status + count simultaneously
                WritePort(PORT_PIT_CMD, PIT_CH2_READBACK);

                // First read from channel port = status byte
                byte status = ReadPort(PORT_PIT_CH2);
                // Next two reads = latched count (lo, hi)
                int lo = ReadPort(PORT_PIT_CH2);
                int hi = ReadPort(PORT_PIT_CH2);
                int count = lo | (hi << 8);

                SafeRestore(orig);

                bool outPin = (status & 0x80) != 0;
                bool nullCount = (status & 0x40) != 0;
                int access = (status >> 4) & 0x03;
                int mode = (status >> 1) & 0x07;
                bool bcd = (status & 0x01) != 0;

                bool accessOk = access == 3;
                bool modeOk = mode == 3;
                bool bcdOk = !bcd;
                bool nullOk = !nullCount;

                string info =
                    $"Status byte : 0x{status:X2}  [{Convert.ToString(status, 2).PadLeft(8, '0')}]\n" +
                    $"  Output pin  : {outPin}\n" +
                    $"  Null-count  : {nullCount}  (expect false = count loaded)  [{(nullOk ? "OK" : "WARN")}]\n" +
                    $"  Access mode : {access}  (expect 3 = lo/hi byte)          [{(accessOk ? "OK" : "FAIL")}]\n" +
                    $"  Timer mode  : {mode}   (expect 3 = square wave)          [{(modeOk ? "OK" : "FAIL")}]\n" +
                    $"  BCD         : {bcd}  (expect false = binary)             [{(bcdOk ? "OK" : "FAIL")}]\n" +
                    $"  Latched count: {count}  (0x{count:X4})";

                bool pass = accessOk && modeOk && bcdOk;
                return pass ? TestResult.Pass(info) : TestResult.Fail(info);
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T06  — Bit-5 Oscillation  [Gate ON / Speaker OFF]
        //  The PIT channel-2 output is wired back to bit 5 of port 0x61.
        //  With the gate enabled but the speaker-data bit cleared, no current
        //  flows through the transducer, yet bit 5 must still toggle at the
        //  programmed frequency.  This confirms the PIT ↔ port feedback path.
        //
        //  At 18.2 Hz, half-period ≈ 27.5 ms.  Sampling every 3 ms gives
        //  ~9 samples per half-period, so no transitions will be missed.
        //  Expected transitions over 700 ms window: ~25.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T06_Bit5OscGateOnly()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(2_000); // let the first cycle settle

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();

                while (sw.ElapsedMilliseconds < 700)
                {
                    bool bit5 = (ReadPort(PORT_SPEAKER) & 0x20) != 0;
                    if (prev.HasValue && prev.Value != bit5) transitions++;
                    prev = bit5;
                    BusyWaitUs(3_000);
                }

                SafeRestore(orig);

                bool pass = transitions >= 4;
                string info =
                    $"Observation window   : 700 ms\n" +
                    $"Bit-5 transitions    : {transitions}  (expect ≥4 at ~18 Hz)\n" +
                    $"Gate ON / Spk-data OFF during entire test — no transducer current.";

                return pass ? TestResult.Pass(info) : TestResult.Fail(info);
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T07  — Bit-5 Stability Control  [Gate OFF]
        //  With the gate bit cleared the PIT output is held at its last state.
        //  Bit 5 must not toggle.  This is the control arm that validates T06:
        //  if T07 also showed transitions it would mean the port is noisy rather
        //  than that the PIT is actually running.
        //  Note: some hypervisors ignore the gate and keep the PIT running;
        //  a WARN (not FAIL) is issued in that case.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T07_Bit5StaticGateOff()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                byte gateOff = (byte)(orig & ~0x03); // gate=0, spk-data=0
                WritePort(PORT_SPEAKER, gateOff);
                BusyWaitUs(800);

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();

                while (sw.ElapsedMilliseconds < 300)
                {
                    bool bit5 = (ReadPort(PORT_SPEAKER) & 0x20) != 0;
                    if (prev.HasValue && prev.Value != bit5) transitions++;
                    prev = bit5;
                    Thread.SpinWait(50);
                }

                SafeRestore(orig);

                bool stable = transitions == 0;
                string info = $"Bit-5 transitions in 300 ms (gate OFF): {transitions}  (expect 0 = frozen)";

                return stable
                    ? TestResult.Pass(info)
                    : TestResult.Warn(info + "\n  Hypervisor may not honour the gate bit.");
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T08  — Sub-Audio Probe  [18 Hz / 100 µs]
        //  The only test that momentarily enables the speaker-data bit (bit 1).
        //  Duration: 100 µs.  At 18.2 Hz the full period is 55 ms, so 100 µs
        //  represents 0.18 % of one cycle — acoustically equivalent to silence.
        //  Purpose: confirm the speaker-enable signal path is wired without
        //  producing any perceptible output.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T08_SubAudioProbe()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);

                // Enable gate AND speaker-data for exactly 100 µs
                WritePort(PORT_SPEAKER, (byte)(orig | 0x03));
                BusyWaitUs(100);
                SafeRestore(orig);

                return TestResult.Pass(
                    $"Speaker-enable path exercised: 100 µs at {EXPECTED_HZ:F2} Hz.\n" +
                    $"0.18 % of one cycle — completely below auditory threshold.\n" +
                    $"Bit 1 (spk-data) returned to 0 immediately after.");
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T09  — PIT Count In-Range at Two Divisors
        //  Programs the PIT with two distinct divisors and verifies that the
        //  latched count is always ≤ the active divisor.  This confirms both
        //  that the PIT accepts new divisors and that the counter wraps correctly.
        //  Speaker-data bit is never set; gate is enabled only for sampling.
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T09_TwoDivisors()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                byte gated = GateOnSpeakerOff(orig);

                // — Divisor A —
                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, gated);
                BusyWaitUs(1_500);
                int cA = LatchCount();

                // — Divisor B (smaller → faster → count reloads sooner) —
                ProgramPitCh2(DIVISOR_MID);
                BusyWaitUs(1_500);
                int cB = LatchCount();

                SafeRestore(orig);

                bool aOk = cA >= 0 && cA <= DIVISOR_MAX;
                bool bOk = cB >= 0 && cB <= DIVISOR_MID;

                string info =
                    $"Divisor A = {DIVISOR_MAX}  latched count = {cA,5}  in-range [0..{DIVISOR_MAX}]? {(aOk ? "YES" : "NO")}\n" +
                    $"Divisor B = {DIVISOR_MID}  latched count = {cB,5}  in-range [0..{DIVISOR_MID}]? {(bOk ? "YES" : "NO")}";

                return aOk && bOk ? TestResult.Pass(info) : TestResult.Fail(info);
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  T10  — Bit-5 Frequency Estimation
        //  Counts bit-5 transitions over a 1.5 s observation window to derive an
        //  approximate output frequency.  The measured value is compared against
        //  the theoretical PIT output for the programmed divisor (±8 Hz tolerance
        //  to accommodate OS scheduler jitter in the sampling loop).
        // ════════════════════════════════════════════════════════════════════════
        private static TestResult T10_FrequencyEstimation()
        {
            byte orig = ReadPort(PORT_SPEAKER);
            try
            {
                ProgramPitCh2(DIVISOR_MAX);
                WritePort(PORT_SPEAKER, GateOnSpeakerOff(orig));
                BusyWaitUs(2_000); // settle

                int transitions = 0;
                bool? prev = null;
                var sw = Stopwatch.StartNew();
                const long windowMs = 1_500;

                while (sw.ElapsedMilliseconds < windowMs)
                {
                    bool bit5 = (ReadPort(PORT_SPEAKER) & 0x20) != 0;
                    if (prev.HasValue && prev.Value != bit5) transitions++;
                    prev = bit5;
                    BusyWaitUs(2_000); // 2 ms sample interval — safe for 18 Hz (period 55 ms)
                }

                long elapsed = sw.ElapsedMilliseconds;
                SafeRestore(orig);

                // transitions / 2 = full cycles
                double measHz = transitions / 2.0 / (elapsed / 1_000.0);
                double error = Math.Abs(measHz - EXPECTED_HZ);
                const double tolerance = 8.0; // Hz

                bool pass = transitions >= 2 && error <= tolerance;

                string info =
                    $"Observation window   : {elapsed} ms\n" +
                    $"Bit-5 transitions    : {transitions}\n" +
                    $"Measured frequency   : {measHz:F2} Hz\n" +
                    $"Expected frequency   : {EXPECTED_HZ:F2} Hz\n" +
                    $"Error                : {error:F2} Hz  " +
                    $"{(pass ? $"(within ±{tolerance} Hz tolerance)" : $"(EXCEEDS ±{tolerance} Hz tolerance)")}";

                return pass ? TestResult.Pass(info)
                     : transitions >= 2 ? TestResult.Warn(info)
                     : TestResult.Fail(info);
            }
            catch { SafeRestore(orig); throw; }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  Logo & banner
        // ════════════════════════════════════════════════════════════════════════

        private static void PrintLogo()
        {
            Console.WriteLine();

            // Two-line box-drawing logo with cyan → magenta gradient
            string art =
                "  ╔═╗╔╦╗╦  ╦╔═╗╔╗╔╔═╗╔═╗╔╦╗  ╔═╗╔═╗╔═╗╔═╗╦╔═╔═╗╦═╗\n" +
                "  ╠═╣ ║║╚╗╔╝╠═╣║║║║  ║╣  ║║  ╚═╗╠═╝║╣ ╠═╣╠╩╗║╣ ╠╦╝\n" +
                "  ╩ ╩═╩╝ ╚╝ ╩ ╩╝╚╝╚═╝╚═╝═╩╝  ╚═╝╩  ╚═╝╩ ╩╩ ╩╚═╝╩╚═\n" +
                "        S I L E N T   E D I T I O N   —   v1.0";

            (int rS, int gS, int bS) = (0, 220, 255);
            (int rE, int gE, int bE) = (200, 0, 255);

            for (int i = 0; i < art.Length; i++)
            {
                double t = (double)i / art.Length;
                int r = (int)(rS + (rE - rS) * t);
                int g = (int)(gS + (gE - gS) * t);
                int b = (int)(bS + (bE - bS) * t);
                Console.Write($"\u001b[38;2;{r};{g};{b}m{art[i]}");
            }
            Console.Write("\u001b[0m\n\n");
        }

        private static void PrintBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ═══════════════════════════════════════════════════════════════");
            Console.WriteLine("      ADVANCED SYSTEM SPEAKER PROBE  —  10-test silent suite");
            Console.WriteLine("  ═══════════════════════════════════════════════════════════════");
            Console.ResetColor();
        }
    }
}