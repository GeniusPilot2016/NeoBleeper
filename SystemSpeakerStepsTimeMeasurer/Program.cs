// ──────────────────────────────────────────────────────────
//  InpOutx64 P/Invoke wrapper
//  Requires InpOutx64.dll (+ WinIo3 kernel driver) in the
//  same directory as the executable.
// ──────────────────────────────────────────────────────────
using System.Diagnostics;
using System.Runtime.InteropServices;

internal static class InpOut
{
    private const string Dll = "InpOutx64.dll";

    [DllImport(Dll, CallingConvention = CallingConvention.Winapi)]
    public static extern byte Inp32(ushort portAddress);

    [DllImport(Dll, CallingConvention = CallingConvention.Winapi)]
    public static extern void Out32(ushort portAddress, byte data);

    [DllImport(Dll, CallingConvention = CallingConvention.Winapi)]
    public static extern uint IsInpOutDriverOpen();
}

// ──────────────────────────────────────────────────────────
//  I/O port addresses
// ──────────────────────────────────────────────────────────
internal static class Port
{
    public const ushort PIT_CH2_DATA = 0x42;   // PIT channel 2 data (divisor)
    public const ushort PIT_CMD = 0x43;   // PIT mode/command register
    public const ushort KB_PORT_B = 0x61;   // i8042 Port B — speaker gate
}

// ──────────────────────────────────────────────────────────
//  A single timed hardware step
// ──────────────────────────────────────────────────────────
internal record TimedStep(
    string Name,
    long ElapsedNs,
    long ThresholdNs,   // warn when ElapsedNs > ThresholdNs
    bool Success,
    string Detail);

// ──────────────────────────────────────────────────────────
//  Colour helpers
// ──────────────────────────────────────────────────────────
internal static class Con
{
    public static void Fg(ConsoleColor c) => Console.ForegroundColor = c;
    public static void Reset() => Console.ResetColor();
    public static void Write(ConsoleColor c, string s) { Fg(c); Console.Write(s); Reset(); }
    public static void Line(ConsoleColor c, string s) { Fg(c); Console.WriteLine(s); Reset(); }
}

// ──────────────────────────────────────────────────────────
//  Entry point
// ──────────────────────────────────────────────────────────
internal static class Program
{
    private const double BeepHz = 440.0;
    private const int BeepMs = 500;
    private const double PitBaseClock = 1_193_182.0;

    // ── Slow-step thresholds (nanoseconds) ───────────────
    // A stall of ≥ 1 ms during PIT programming or gate
    // switching is long enough to produce an audible
    // artifact (click, chirp, or brief unintended tone)
    // through the PC speaker.
    private const long ThresholdPitCmdNs = 1_000_000;  // 1 ms
    private const long ThresholdPitDivisorNs = 1_000_000;  // 1 ms
    private const long ThresholdGateOpenNs = 1_000_000;  // 1 ms
    private const long ThresholdGateCloseNs = 1_000_000;  // 1 ms

    // ─────────────────────────────────────────────────────
    static int Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        PrintBanner();

        // ════ Pre-flight (not timed) ═════════════════════
        if (!PreFlight(out string pfError))
        {
            Con.Line(ConsoleColor.Red, $"\n  ✗  Pre-flight failed: {pfError}");
            return 1;
        }

        byte originalPortB = InpOut.Inp32(Port.KB_PORT_B);
        ushort divisor = (ushort)Math.Round(PitBaseClock / BeepHz);

        Con.Line(ConsoleColor.DarkGray,
            $"\n  Port 0x61 saved = 0x{originalPortB:X2}\n" +
            $"  PIT divisor     = {divisor} (0x{divisor:X4})" +
            $"  → actual {PitBaseClock / divisor:F2} Hz\n");

        // ════ Timed steps ═════════════════════════════════
        var results = new List<TimedStep>();

        // — Step A: PIT command ——————————————————————————
        results.Add(RunTimed(
            "PIT command register  (0x43 ← 0xB6)",
            ThresholdPitCmdNs,
            () => {
                InpOut.Out32(Port.PIT_CMD, 0xB6);
                return "Channel 2 | lo/hi access | mode 3 square-wave | binary";
            }));

        // — Step B: Divisor low byte ————————————————————
        byte lo = (byte)(divisor & 0xFF);
        results.Add(RunTimed(
            $"PIT ch2 data lo-byte  (0x42 ← 0x{lo:X2})",
            ThresholdPitDivisorNs,
            () => {
                InpOut.Out32(Port.PIT_CH2_DATA, lo);
                return $"Low  byte 0x{lo:X2} of divisor {divisor}";
            }));

        // — Step C: Divisor high byte ———————————————————
        byte hi = (byte)(divisor >> 8);
        results.Add(RunTimed(
            $"PIT ch2 data hi-byte  (0x42 ← 0x{hi:X2})",
            ThresholdPitDivisorNs,
            () => {
                InpOut.Out32(Port.PIT_CH2_DATA, hi);
                return $"High byte 0x{hi:X2} of divisor {divisor}";
            }));

        // — Step D: Open speaker gate ———————————————————
        byte gateOn = (byte)(originalPortB | 0x03);
        results.Add(RunTimed(
            "Speaker gate OPEN     (0x61 |= 0x03)",
            ThresholdGateOpenNs,
            () => {
                InpOut.Out32(Port.KB_PORT_B, gateOn);
                return $"Port 0x61: 0x{originalPortB:X2} → 0x{gateOn:X2}  bits[1:0]=11  🔊 ON";
            }));

        // ════ Beep (not timed) ════════════════════════════
        Con.Line(ConsoleColor.DarkYellow,
            $"\n  … beeping at {BeepHz} Hz for {BeepMs} ms …\n");
        Thread.Sleep(BeepMs);

        // — Step E: Close speaker gate ——————————————————
        byte gateOff = (byte)(originalPortB & 0xFC);
        results.Add(RunTimed(
            "Speaker gate CLOSE    (0x61 &= 0xFC)",
            ThresholdGateCloseNs,
            () => {
                InpOut.Out32(Port.KB_PORT_B, gateOff);
                return $"Port 0x61: 0x{gateOn:X2} → 0x{gateOff:X2}  bits[1:0]=00  🔇 OFF";
            }));

        // ════ Results ═════════════════════════════════════
        Console.WriteLine();
        PrintTable(results);
        PrintWarnings(results);

        Console.WriteLine();
        Con.Line(ConsoleColor.DarkGray, "  Press any key to exit…");
        Console.ReadKey(true);
        return 0;
    }

    // ──────────────────────────────────────────────────────
    //  Run a named step, time it, return a TimedStep record.
    //  Prints a live one-liner while running.
    // ──────────────────────────────────────────────────────
    private static TimedStep RunTimed(string name, long threshNs, Func<string> action)
    {
        Con.Write(ConsoleColor.DarkCyan, $"  ▶  {name} ");

        var sw = Stopwatch.StartNew();
        string detail;
        bool ok;
        try { detail = action(); ok = true; }
        catch (Exception ex) { detail = ex.Message; ok = false; }
        sw.Stop();

        long ns = sw.ElapsedTicks * 1_000_000_000L / Stopwatch.Frequency;
        bool slow = ok && ns > threshNs;

        Con.Write(ok ? ConsoleColor.Green : ConsoleColor.Red, ok ? "✓" : "✗");
        Console.Write(slow ? "  " : "   ");
        if (slow) Con.Write(ConsoleColor.Yellow, "⚠");
        Con.Line(ConsoleColor.DarkGray, $"  ({FormatNs(ns)})");

        if (!string.IsNullOrEmpty(detail))
            Con.Line(ok ? ConsoleColor.Gray : ConsoleColor.Red, $"       {detail}");

        return new TimedStep(name, ns, threshNs, ok, detail);
    }

    // ──────────────────────────────────────────────────────
    //  Summary table — hardware-only steps
    // ──────────────────────────────────────────────────────
    private static void PrintTable(List<TimedStep> steps)
    {
        const int nw = 46;   // name column width
        const int tw = 14;   // elapsed column width
        const int sw = 9;   // status column width

        static string B(int w) => new string('═', w);
        string top = "  ╔" + B(nw) + "╦" + B(tw) + "╦" + B(sw) + "╗";
        string mid = "  ╠" + B(nw) + "╬" + B(tw) + "╬" + B(sw) + "╣";
        string bot = "  ╚" + B(nw) + "╩" + B(tw) + "╩" + B(sw) + "╝";

        Con.Line(ConsoleColor.Yellow, top);
        Con.Line(ConsoleColor.Yellow,
            $"  ║{"  HARDWARE STEP TIMING",-nw}║{"  Elapsed",tw}║{"  Status",sw}║");
        Con.Line(ConsoleColor.Yellow, mid);

        char[] ids = { 'A', 'B', 'C', 'D', 'E' };
        long total = 0;

        for (int i = 0; i < steps.Count; i++)
        {
            var s = steps[i];
            bool slow = s.Success && s.ElapsedNs > s.ThresholdNs;
            total += s.ElapsedNs;

            Con.Write(ConsoleColor.Yellow, "  ║");
            Con.Write(ConsoleColor.Cyan, $"  [{ids[i]}] {s.Name,-(nw - 6)}");
            Con.Write(ConsoleColor.Yellow, "║");
            Con.Write(ConsoleColor.White, $"{FormatNs(s.ElapsedNs),tw}");
            Con.Write(ConsoleColor.Yellow, "║");

            if (!s.Success) Con.Write(ConsoleColor.Red, $"{"  FAIL",sw}");
            else if (slow) Con.Write(ConsoleColor.Yellow, $"{"  SLOW ⚠",sw}");
            else Con.Write(ConsoleColor.Green, $"{"  OK",sw}");

            Con.Line(ConsoleColor.Yellow, "║");
        }

        Con.Line(ConsoleColor.Yellow, mid);
        Con.Write(ConsoleColor.Yellow, "  ║");
        Con.Write(ConsoleColor.Magenta, $"{"  TOTAL (5 timed steps)",-nw}");
        Con.Write(ConsoleColor.Yellow, "║");
        Con.Write(ConsoleColor.Magenta, $"{FormatNs(total),tw}");
        Con.Write(ConsoleColor.Yellow, "║");
        Con.Write(ConsoleColor.Green, $"{"  ✓",sw}");
        Con.Line(ConsoleColor.Yellow, "║");
        Con.Line(ConsoleColor.Yellow, bot);
    }

    // ──────────────────────────────────────────────────────
    //  Warning block — only printed when ≥1 step is slow
    // ──────────────────────────────────────────────────────
    private static void PrintWarnings(List<TimedStep> steps)
    {
        char[] ids = { 'A', 'B', 'C', 'D', 'E' };
        bool anyWarn = false;

        for (int i = 0; i < steps.Count; i++)
        {
            var s = steps[i];
            if (!s.Success || s.ElapsedNs <= s.ThresholdNs) continue;

            if (!anyWarn)
            {
                Console.WriteLine();
                Con.Line(ConsoleColor.Yellow,
                    "  ╔══════════════════════════════════════════════════════════════╗");
                Con.Line(ConsoleColor.Yellow,
                    "  ║  ⚠  SLOW STEP WARNING(S)                                    ║");
                Con.Line(ConsoleColor.Yellow,
                    "  ╠══════════════════════════════════════════════════════════════╣");
                anyWarn = true;
            }

            double ratio = (double)s.ElapsedNs / s.ThresholdNs;
            Con.Line(ConsoleColor.Yellow, "  ║");
            Con.Write(ConsoleColor.Yellow, "  ║  ");
            Con.Line(ConsoleColor.DarkYellow,
                $"Step [{ids[i]}] took {FormatNs(s.ElapsedNs)}" +
                $"  (threshold {FormatNs(s.ThresholdNs)},  {ratio:F1}× over limit)");
            Con.Write(ConsoleColor.Yellow, "  ║  ");
            Con.Line(ConsoleColor.DarkGray, ids[i] switch
            {
                'A' =>
                    "⟶ PIT command written late — channel 2 may have been left in an undefined mode,\n" +
                    "  ║     producing noise before the divisor was loaded.\n" +
                    "  ║     Cause: SMI/SMM latency, PCH bus contention, or hypervisor exit.",
                'B' =>
                    "⟶ Divisor low byte delayed — PIT may have latched a garbage half-word\n" +
                    "  ║     and ticked at an arbitrary frequency for ~1 ms (audible chirp).\n" +
                    "  ║     Cause: SMI/SMM interrupt between the two 0x42 writes.",
                'C' =>
                    "⟶ Divisor high byte delayed — same risk as step B; the PIT ran with\n" +
                    "  ║     only the low byte loaded for ≥1 ms before the high byte arrived.\n" +
                    "  ║     Cause: SMI/SMM interrupt between the two 0x42 writes.",
                'D' =>
                    "⟶ Gate-open stall — speaker was gated on but the divisor write may\n" +
                    "  ║     not have propagated yet, causing ~1 ms of unintended tone or click.\n" +
                    "  ║     Cause: i8042 controller stall, SMI interrupt, or driver jitter.",
                'E' =>
                    "⟶ Gate-close stall — speaker kept playing for ≥1 ms past the intended\n" +
                    "  ║     stop point, producing an audible tail/click on beep end.\n" +
                    "  ║     Cause: i8042 controller stall, SMI interrupt, or driver jitter.",
                _ => "Unknown cause."
            });
        }

        if (anyWarn)
        {
            Con.Line(ConsoleColor.Yellow, "  ║");
            Con.Line(ConsoleColor.Yellow,
                "  ╚══════════════════════════════════════════════════════════════╝");
        }
        else
        {
            Console.WriteLine();
            Con.Write(ConsoleColor.Green, "  ✓  ");
            Con.Line(ConsoleColor.DarkGreen,
                "All hardware steps completed well under 1 ms — no audible artifacts expected.");
        }
    }

    // ──────────────────────────────────────────────────────
    //  Platform / driver check (not timed)
    // ──────────────────────────────────────────────────────
    private static bool PreFlight(out string error)
    {
        error = string.Empty;

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        { error = "Windows required for direct port I/O."; return false; }

        if (!Environment.Is64BitProcess)
        { error = "Must run as a 64-bit process."; return false; }

        Con.Write(ConsoleColor.DarkGray, "  Loading InpOutx64.dll … ");
        uint ok = InpOut.IsInpOutDriverOpen();
        if (ok == 0)
        {
            error = "IsInpOutDriverOpen() returned 0. " +
                    "Run as Administrator and ensure InpOutx64.dll is present.";
            return false;
        }
        Con.Line(ConsoleColor.Green, $"OK  (driver handle = {ok})");
        return true;
    }

    // ──────────────────────────────────────────────────────
    //  Format nanoseconds to a human-readable string
    // ──────────────────────────────────────────────────────
    private static string FormatNs(long ns) => ns switch
    {
        < 1_000 => $"{ns} ns",
        < 1_000_000 => $"{ns / 1_000.0:F2} µs",
        < 1_000_000_000 => $"{ns / 1_000_000.0:F3} ms",
        _ => $"{ns / 1_000_000_000.0:F3} s"
    };

    // ──────────────────────────────────────────────────────
    //  ASCII art banner
    // ──────────────────────────────────────────────────────
    private static void PrintBanner()
    {
        Console.Clear();
        Con.Line(ConsoleColor.Cyan,
            "\n" +
            @"  ██████╗  ██████╗    ███████╗██████╗ ███████╗ █████╗ ██╗  ██╗███████╗██████╗ " + "\n" +
            @"  ██╔══██╗██╔════╝    ██╔════╝██╔══██╗██╔════╝██╔══██╗██║ ██╔╝██╔════╝██╔══██╗" + "\n" +
            @"  ██████╔╝██║         ███████╗██████╔╝█████╗  ███████║█████╔╝ █████╗  ██████╔╝" + "\n" +
            @"  ██╔═══╝ ██║         ╚════██║██╔═══╝ ██╔══╝  ██╔══██║██╔═██╗ ██╔══╝  ██╔══██╗" + "\n" +
            @"  ██║     ╚██████╗    ███████║██║     ███████╗██║  ██║██║  ██╗███████╗██║  ██║" + "\n" +
            @"  ╚═╝      ╚═════╝    ╚══════╝╚═╝     ╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝");
        Con.Line(ConsoleColor.DarkCyan,
            "\n" +
            @"  ┌───────────────────────────────────────────────────────────────────────────┐" + "\n" +
            @"  │  Direct PC Speaker Beep  •  InpOutx64  •  Hardware-Step Timing            │" + "\n" +
            @"  │  Timed: PIT prep (0x43/0x42)  │  Gate open (0x61)  │  Gate close (0x61)   │" + "\n" +
            @"  └───────────────────────────────────────────────────────────────────────────┘");
        Con.Line(ConsoleColor.DarkGray,
            "\n" +
            "  Steps A–C  program 8254 PIT channel 2 for a square-wave at the target freq.\n" +
            "  Step  D    opens  the speaker gate (Port 0x61 bits[1:0] = 11).\n" +
            "  Step  E    closes the speaker gate (Port 0x61 bits[1:0] = 00).\n" +
            "  Threshold  1 ms per step — a stall ≥ 1 ms produces an audible artifact.\n" +
            "  Typical    port OUT latency is 0.5–3 µs on bare metal (well within limit).");
        Con.Line(ConsoleColor.DarkYellow,
            "\n  ⚠  Requires: Administrator privileges + InpOutx64.dll in working dir\n");
        Con.Line(ConsoleColor.White, "  Hardware steps\n  " + new string('─', 74) + "\n");
    }
}