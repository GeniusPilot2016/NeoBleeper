using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace NeoBleeperSpeakerExistenceTest
{
    internal static class Program
    {
        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Out32(short portAddress, short data);

        [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
        private static extern short Inp32(short portAddress);

        private const short PortSpeakerControl = 0x61;
        private const short PortPitControl = 0x43;
        private const short PortPitChannel2 = 0x42;

        // Very short probe to minimize noticeability.
        // Still not guaranteed to be inaudible on every machine.
        private const int ProbeFrequencyHz = 9000;
        private const int ProbeDurationMs = 1;

        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            PrintColorASCIIArtLogo();
            PrintHeader();

            var result = DetectSystemSpeaker();

            PrintResult(result);
        }

        private static DetectionResult DetectSystemSpeaker()
        {
            var result = new DetectionResult();

            if (!TryReadPort61(out byte original61, out string readMessage))
            {
                result.Exists = false;
                result.Log.Add(readMessage);
                result.Reason = "Port 0x61 could not be read.";
                return result;
            }

            result.Log.Add(readMessage);
            result.Log.Add("Initial port 0x61 state:");
            result.Log.Add(DescribePort61(original61));

            bool roundTripOk = CheckControlPortRoundTrip(out string rtMessage);
            result.Log.Add(rtMessage);
            result.ControlPortRoundTripOk = roundTripOk;

            bool pitBit5Ok = CheckPitChannel2Bit5Activity(
                frequencyHz: ProbeFrequencyHz,
                sampleCount: 64,
                out int transitions,
                out string pitMessage);

            result.Log.Add(pitMessage);
            result.Bit5Transitions = transitions;
            result.PitBit5ActivityOk = pitBit5Ok;

            bool minimalProbeOk = TryMinimalAudibleProbe(
                ProbeFrequencyHz,
                ProbeDurationMs,
                out string probeMessage);

            result.Log.Add(probeMessage);
            result.MinimalProbeExecuted = minimalProbeOk;

            // Strict decision:
            // We only say EXISTS if:
            // - port works
            // - round trip works
            // - PIT channel 2 seems alive via bit 5 transitions
            // - minimal audible probe path executed successfully
            //
            // This is intentionally stricter than your old logic to avoid false positives.
            result.Exists = roundTripOk && pitBit5Ok && minimalProbeOk;

            result.Reason = result.Exists
                ? "Legacy speaker path appears to be fully implemented."
                : "Could not confirm a usable legacy speaker output path.";

            return result;
        }

        private static bool TryReadPort61(out byte value, out string message)
        {
            try
            {
                value = ReadPortByte(PortSpeakerControl);
                message = $"Read port 0x61 successfully: 0x{value:X2}";
                return true;
            }
            catch (Exception ex)
            {
                value = 0;
                message = $"Failed to read port 0x61: {ex.Message}";
                return false;
            }
        }

        private static bool CheckControlPortRoundTrip(out string details)
        {
            byte originalState = 0;

            try
            {
                originalState = ReadPortByte(PortSpeakerControl);

                byte safeBase = (byte)(originalState & ~0x02);
                byte variantA = (byte)(safeBase & ~0x01);
                byte variantB = (byte)(safeBase | 0x01);

                WritePortByte(PortSpeakerControl, variantA);
                Thread.Sleep(1);
                byte readA = ReadPortByte(PortSpeakerControl);

                WritePortByte(PortSpeakerControl, variantB);
                Thread.Sleep(1);
                byte readB = ReadPortByte(PortSpeakerControl);

                RestoreSpeakerControl(originalState);

                bool aMatched = ((readA & 0x01) == (variantA & 0x01)) && ((readA & 0x02) == 0);
                bool bMatched = ((readB & 0x01) == (variantB & 0x01)) && ((readB & 0x02) == 0);

                details =
                    "Control-port round trip:\n" +
                    $"  Original: 0x{originalState:X2}\n" +
                    $"  WriteA  : 0x{variantA:X2} -> ReadA: 0x{readA:X2}\n" +
                    $"  WriteB  : 0x{variantB:X2} -> ReadB: 0x{readB:X2}\n" +
                    $"  Result  : {(aMatched && bMatched ? "PASS" : "FAIL")}";

                return aMatched && bMatched;
            }
            catch (Exception ex)
            {
                RestoreSpeakerControl(originalState);
                details = $"Control-port round trip failed: {ex.Message}";
                return false;
            }
        }

        private static bool CheckPitChannel2Bit5Activity(
            int frequencyHz,
            int sampleCount,
            out int transitions,
            out string details)
        {
            byte original61 = 0;

            try
            {
                original61 = ReadPortByte(PortSpeakerControl);

                int divisor = 1193182 / Math.Max(1, frequencyHz);
                if (divisor < 1) divisor = 1;
                if (divisor > 65535) divisor = 65535;

                // Program PIT channel 2, mode 3 square wave, lobyte/hibyte.
                WritePortByte(PortPitControl, 0xB6);
                WritePortByte(PortPitChannel2, (byte)(divisor & 0xFF));
                WritePortByte(PortPitChannel2, (byte)((divisor >> 8) & 0xFF));

                // Gate timer 2 ON, keep speaker data OFF.
                byte gated = (byte)((original61 | 0x01) & ~0x02);
                WritePortByte(PortSpeakerControl, gated);
                Thread.Sleep(1);

                transitions = 0;
                bool? prev = null;

                for (int i = 0; i < sampleCount; i++)
                {
                    byte sample = ReadPortByte(PortSpeakerControl);
                    bool bit5 = (sample & 0x20) != 0;

                    if (prev.HasValue && prev.Value != bit5)
                        transitions++;

                    prev = bit5;

                    Thread.Sleep(0);
                }

                RestoreSpeakerControl(original61);

                bool ok = transitions > 0;

                details =
                    "PIT channel 2 / bit 5 activity test:\n" +
                    $"  Frequency      : {frequencyHz} Hz\n" +
                    $"  Divisor        : {divisor}\n" +
                    $"  Bit5 changes   : {transitions}\n" +
                    $"  Result         : {(ok ? "PASS" : "FAIL")}";

                return ok;
            }
            catch (Exception ex)
            {
                RestoreSpeakerControl(original61);
                transitions = 0;
                details = $"PIT bit 5 activity test failed: {ex.Message}";
                return false;
            }
        }

        private static bool TryMinimalAudibleProbe(
            int frequencyHz,
            int durationMs,
            out string details)
        {
            byte original61 = 0;

            try
            {
                original61 = ReadPortByte(PortSpeakerControl);

                int divisor = 1193182 / Math.Max(1, frequencyHz);
                if (divisor < 1) divisor = 1;
                if (divisor > 65535) divisor = 65535;

                // Program PIT channel 2.
                WritePortByte(PortPitControl, 0xB6);
                WritePortByte(PortPitChannel2, (byte)(divisor & 0xFF));
                WritePortByte(PortPitChannel2, (byte)((divisor >> 8) & 0xFF));

                // Enable gate + speaker only for a tiny duration.
                byte onState = (byte)(original61 | 0x03);
                WritePortByte(PortSpeakerControl, onState);

                BusyWaitMilliseconds(durationMs);

                RestoreSpeakerControl(original61);

                details =
                    "Minimal audible probe executed:\n" +
                    $"  Frequency : {frequencyHz} Hz\n" +
                    $"  Duration  : {durationMs} ms\n" +
                    $"  Result    : EXECUTED";

                return true;
            }
            catch (Exception ex)
            {
                RestoreSpeakerControl(original61);
                details = $"Minimal audible probe failed: {ex.Message}";
                return false;
            }
        }

        private static void BusyWaitMilliseconds(int ms)
        {
            if (ms <= 0)
                return;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < ms)
            {
            }
        }

        private static byte ReadPortByte(short port)
        {
            return unchecked((byte)(Inp32(port) & 0xFF));
        }

        private static void WritePortByte(short port, byte value)
        {
            Out32(port, value);
        }

        private static void RestoreSpeakerControl(byte originalState)
        {
            try
            {
                WritePortByte(PortSpeakerControl, originalState);
                Thread.Sleep(1);
            }
            catch
            {
            }
        }

        private static string DescribePort61(byte value)
        {
            return
                $"  Hex    : 0x{value:X2}\n" +
                $"  Binary : {Convert.ToString(value, 2).PadLeft(8, '0')}\n" +
                $"  Bit 0  : {(((value & 0x01) != 0) ? "1" : "0")} (Timer 2 gate)\n" +
                $"  Bit 1  : {(((value & 0x02) != 0) ? "1" : "0")} (Speaker data enable)\n" +
                $"  Bit 5  : {(((value & 0x20) != 0) ? "1" : "0")} (Timer 2 out / feedback)";
        }

        private static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("====================================================");
            Console.WriteLine("        STRICT SYSTEM SPEAKER EXISTENCE TEST");
            Console.WriteLine("====================================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Probe chirp: {ProbeFrequencyHz} Hz for {ProbeDurationMs} ms");
            Console.WriteLine("Designed to be barely noticeable, but not guaranteed inaudible.");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void PrintResult(DetectionResult result)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Diagnostics");
            Console.WriteLine("-----------");
            foreach (string line in result.Log)
                Console.WriteLine(line);

            Console.WriteLine();

            Console.Write("Result: ");
            Console.ForegroundColor = result.Exists ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(result.Exists ? "EXISTS" : "NOT EXISTS");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(result.Reason);
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Note:");
            Console.WriteLine("This test is designed to find out if hidden or explicit system speaker hardware is present and functional via legacy ports. It does not guarantee that every possible speaker configuration will be detected, especially if non-standard hardware or drivers are involved.");
            Console.ResetColor();
        }

        private static void PrintColorASCIIArtLogo()
        {
            Console.WriteLine();
            string ASCIIArt = "   _____           _                    _____                  _             \r\n  / ____|         | |                  / ____|                | |            \r\n | (___  _   _ ___| |_ ___ _ __ ___   | (___  _ __   ___  __ _| | _____ _ __ \r\n  \\___ \\| | | / __| __/ _ \\ '_ ` _ \\   \\___ \\| '_ \\ / _ \\/ _` | |/ / _ \\ '__|\r\n  ____) | |_| \\__ \\ ||  __/ | | | | |  ____) | |_) |  __/ (_| |   <  __/ |   \r\n |_____/ \\__, |___/\\__\\___|_| |_| |_|_|_____/| .__/ \\___|\\__,_|_|\\_\\___|_|   \r\n  / ____| __/ |                     |__   __|| |   | |                       \r\n | (___  |___/_ __  ___  ___  _ __     | | __|_|___| |_                      \r\n  \\___ \\ / _ \\ '_ \\/ __|/ _ \\| '__|    | |/ _ \\/ __| __|                     \r\n  ____) |  __/ | | \\__ \\ (_) | |       | |  __/\\__ \\ |_                      \r\n |_____/ \\___|_| |_|___/\\___/|_|       |_|\\___||___/\\__|                     \r\n                                                                             \r\n                                                                             ";

            // 2-color gradient from Cyan to Magenta
            int rStart = 0, gStart = 255, bStart = 255;
            int rEnd = 255, gEnd = 0, bEnd = 255;

            for (int i = 0; i < ASCIIArt.Length; i++)
            {
                double t = (double)i / ASCIIArt.Length;
                int r = (int)(rStart + (rEnd - rStart) * t);
                int g = (int)(gStart + (gEnd - gStart) * t);
                int b = (int)(bStart + (bEnd - bStart) * t);
                Console.Write($"\u001b[38;2;{r};{g};{b}m{ASCIIArt[i]}");
            }
            Console.Write("\u001b[0m");
            Console.WriteLine();

            Console.ResetColor();
            Console.WriteLine();
        }

        private sealed class DetectionResult
        {
            public bool Exists { get; set; }
            public bool ControlPortRoundTripOk { get; set; }
            public bool PitBit5ActivityOk { get; set; }
            public bool MinimalProbeExecuted { get; set; }
            public int Bit5Transitions { get; set; }
            public string Reason { get; set; } = "";
            public List<string> Log { get; } = new();
        }
    }
}