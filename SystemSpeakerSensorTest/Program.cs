using System.Runtime.InteropServices;

namespace SystemSpeakerSensorTest
{
    internal class Program
    {
        [DllImport("inpoutx64.dll")]
        extern static void Out32(short PortAddress, short Data);
        [DllImport("inpoutx64.dll")]
        extern static char Inp32(short PortAddress);

        // Ultrasonik frekans kullan (duyulamaz)
        private const int ULTRASONIC_FREQ = 25000; // 25 kHz - insan işitme aralığının dışında
        private const int ALTERNATIVE_ULTRASONIC_FREQ = 30000; // 30 kHz - alternatif
        private const int PIT_BASE_FREQ = 1193180; // PIT temel frekansı

        public static bool CheckElectricalFeedbackOnPort()
        {
            try
            {
                // Ultrasonik frekans kullan
                Out32(0x43, 0xB6);
                int div = PIT_BASE_FREQ / ULTRASONIC_FREQ;
                Out32(0x42, (byte)(div & 0xFF));
                Out32(0x42, (byte)(div >> 8));

                byte originalState = (byte)Inp32(0x61);

                // Speaker gate'i aç ve bekle
                Out32(0x61, (byte)(originalState | 0x03));
                Thread.Sleep(50); // Ultrasonik frekanslar çok hızlı, daha kısa süre yeterli

                List<byte> enabledSamples = new List<byte>();
                for (int i = 0; i < 10; i++) // Daha fazla örnek al
                {
                    enabledSamples.Add((byte)Inp32(0x61));
                    Thread.Sleep(1);
                }
                byte stateEnabled = enabledSamples[enabledSamples.Count / 2];

                // Speaker gate'i kapat ve bekle
                Out32(0x61, (byte)(originalState & 0xFC));
                Thread.Sleep(50);

                List<byte> disabledSamples = new List<byte>();
                for (int i = 0; i < 10; i++)
                {
                    disabledSamples.Add((byte)Inp32(0x61));
                    Thread.Sleep(1);
                }
                byte stateDisabled = disabledSamples[disabledSamples.Count / 2];

                // Orijinal durumu geri yükle
                Out32(0x61, originalState);
                Thread.Sleep(10);

                bool bit5VariesWhenEnabled = enabledSamples.Select(s => (byte)(s & 0x20)).Distinct().Count() > 1;
                bool feedbackPresent = ((stateEnabled & 0x20) != (stateDisabled & 0x20)) || bit5VariesWhenEnabled;
                bool gateResponsive = ((stateEnabled & 0x03) == 0x03) && ((stateDisabled & 0x03) == 0x00);

                return feedbackPresent && gateResponsive;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking electrical feedback on port 0x61: {ex.Message}");
                return false;
            }
        }

        public static bool CheckPortStateStability()
        {
            try
            {
                // Ultrasonik frekans kullan
                Out32(0x43, 0xB6);
                int div = PIT_BASE_FREQ / ULTRASONIC_FREQ;
                Out32(0x42, (byte)(div & 0xFF));
                Out32(0x42, (byte)(div >> 8));

                byte originalState = (byte)Inp32(0x61);
                Out32(0x61, (byte)(originalState | 0x03));

                Thread.Sleep(50);

                // Ultrasonik frekansta çok hızlı örnekleme gerekli
                List<byte> samples = new List<byte>();
                for (int i = 0; i < 100; i++)
                {
                    samples.Add((byte)Inp32(0x61));
                    // Gecikme yok veya çok minimal - 25 kHz = 0.04ms periyot
                }

                Out32(0x61, originalState);
                Thread.Sleep(10);

                var bit5Values = samples.Select(s => (byte)(s & 0x20)).Distinct().ToList();
                bool bit5Varies = bit5Values.Count > 1;

                int bit5Transitions = 0;
                for (int i = 0; i < samples.Count - 1; i++)
                {
                    if ((samples[i] & 0x20) != (samples[i + 1] & 0x20))
                    {
                        bit5Transitions++;
                    }
                }

                bool sufficientTransitions = bit5Transitions >= 3; // Ultrasonik frekansta daha az geçiş yeterli

                return bit5Varies && sufficientTransitions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking port state stability: {ex.Message}");
                return false;
            }
        }

        public static bool AdvancedFrequencySweepTest()
        {
            try
            {
                Console.WriteLine("\n=== Advanced Frequency Sweep Test (Ultrasonic) ===");
                byte originalState = (byte)Inp32(0x61);
                bool anyFrequencyWorks = false;

                // Sadece ultrasonik frekanslar kullan (20 kHz+)
                int[] testFrequencies = { 20000, 25000, 30000, 35000, 40000 };

                foreach (int freq in testFrequencies)
                {
                    int div = PIT_BASE_FREQ / freq;
                    if (div < 1) continue; // Çok yüksek frekans, atla

                    Out32(0x43, 0xB6);
                    Out32(0x42, (byte)(div & 0xFF));
                    Out32(0x42, (byte)(div >> 8));

                    Out32(0x61, (byte)(originalState | 0x03));
                    Thread.Sleep(30); // Ultrasonik frekanslarda daha kısa yeterli

                    List<byte> samples = new List<byte>();
                    for (int i = 0; i < 50; i++)
                    {
                        samples.Add((byte)Inp32(0x61));
                        // Gecikme minimal veya yok
                    }

                    int transitions = 0;
                    for (int i = 0; i < samples.Count - 1; i++)
                    {
                        if ((samples[i] & 0x20) != (samples[i + 1] & 0x20))
                            transitions++;
                    }

                    Console.WriteLine($"  {freq} Hz: {transitions} transitions (SILENT - ultrasonic)");

                    if (transitions >= 2) // Ultrasonik frekansta daha toleranslı
                    {
                        anyFrequencyWorks = true;
                        break;
                    }
                }

                Out32(0x61, originalState);
                Thread.Sleep(10);

                return anyFrequencyWorks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in frequency sweep test: {ex.Message}");
                return false;
            }
        }

        public static bool IsFunctionalSystemSpeaker()
        {
            bool electricalFeedbackValid = CheckElectricalFeedbackOnPort();
            bool portStateStable = CheckPortStateStability();
            bool frequencySweepWorks = AdvancedFrequencySweepTest();

            return electricalFeedbackValid || portStateStable || frequencySweepWorks;
        }

        public static void LogSystemSpeakerStatus()
        {
            Console.WriteLine("=== System Speaker Detection Test (SILENT MODE) ===\n");
            Console.WriteLine($"Using ultrasonic frequency: {ULTRASONIC_FREQ} Hz (inaudible to humans)\n");

            byte initialState = (byte)Inp32(0x61);
            Console.WriteLine($"Initial port 0x61 state: 0x{initialState:X2} (binary: {Convert.ToString(initialState, 2).PadLeft(8, '0')})");
            Console.WriteLine($"  Bit 0 (Timer 2 gate): {(initialState & 0x01) != 0}");
            Console.WriteLine($"  Bit 1 (Speaker data): {(initialState & 0x02) != 0}");
            Console.WriteLine($"  Bit 5 (Timer 2 out): {(initialState & 0x20) != 0}\n");

            bool electricalFeedbackValid = CheckElectricalFeedbackOnPort();
            Console.WriteLine($"Electrical feedback valid: {electricalFeedbackValid}");

            bool portStateStable = CheckPortStateStability();
            Console.WriteLine($"Port state stable: {portStateStable}");

            bool frequencySweepWorks = AdvancedFrequencySweepTest();
            Console.WriteLine($"Frequency sweep test: {frequencySweepWorks}\n");

            if (electricalFeedbackValid || portStateStable || frequencySweepWorks)
            {
                Console.WriteLine("✓ Functional system speaker or buzzer detected (SILENTLY).");
            }
            else
            {
                Console.WriteLine("✗ No functional system speaker or buzzer detected.");
                Console.WriteLine("\nPossible reasons:");
                Console.WriteLine("  - System speaker hardware not present");
                Console.WriteLine("  - Speaker disabled in BIOS/UEFI");
                Console.WriteLine("  - Port access restricted by OS/driver");
                Console.WriteLine("  - Virtual machine with no speaker emulation");
            }
        }

        public static void DebugPortBehavior()
        {
            Console.WriteLine("\n=== Debug: Detailed Port Behavior (Ultrasonic) ===");

            Out32(0x43, 0xB6);
            int div = PIT_BASE_FREQ / ULTRASONIC_FREQ;
            Out32(0x42, (byte)(div & 0xFF));
            Out32(0x42, (byte)(div >> 8));

            byte originalState = (byte)Inp32(0x61);
            Out32(0x61, (byte)(originalState | 0x03));
            Thread.Sleep(50);

            Console.WriteLine($"Sampling with {ULTRASONIC_FREQ} Hz (silent frequency):");
            for (int i = 0; i < 50; i++)
            {
                byte sample = (byte)Inp32(0x61);
                Console.WriteLine($"Sample {i:D2}: 0x{sample:X2} | Bit5={((sample & 0x20) != 0 ? "1" : "0")} Bits0-1={sample & 0x03:X}");
            }

            Out32(0x61, originalState);
        }

        static void Main()
        {
            LogSystemSpeakerStatus();
            //DebugPortBehavior();
        }
    }
}