using System.Runtime.InteropServices;

namespace SystemSpeakerSensorTest
{
    /// <summary>
    /// Provides methods for detecting and testing the presence and functionality of the system speaker or buzzer using
    /// silent ultrasonic frequencies. Intended for use in development and diagnostics of system speaker sensors,
    /// particularly for environments where audible output is undesirable.
    /// </summary>
    /// <remarks>This class interacts directly with hardware I/O ports to perform low-level tests on the
    /// system speaker, including electrical feedback checks, port state stability analysis, and ultrasonic frequency
    /// sweeps. All detection methods are designed to minimize audible clicks or sounds by using frequencies above the
    /// human hearing range and gradual state transitions. Requires access to the inpoutx64.dll native library and
    /// sufficient privileges to access hardware ports; may not function in virtualized environments or on systems where
    /// direct port access is restricted. Thread safety is not guaranteed, and concurrent access to hardware ports
    /// should be avoided.</remarks>
    internal class Program // Console program for testing silent system speaker detection using ultrasonic frequencies to help development of system speaker sensor in NeoBleeper
    {
        [DllImport("inpoutx64.dll")]
        extern static void Out32(short PortAddress, short Data);
        [DllImport("inpoutx64.dll")]
        extern static char Inp32(short PortAddress);

        private const int ULTRASONIC_FREQ = 30000; // 30 kHz - high enough to be inaudible to users
        private const int PIT_BASE_FREQ = 1193180;

        /// <summary>
        /// Gradually enables the system speaker using the specified original port state to minimize audible clicks during
        /// activation.
        /// </summary>
        /// <remarks>This method performs a soft enable sequence for the system speaker, reducing the
        /// likelihood of producing audible clicks. It is intended for scenarios where minimizing sound artifacts during
        /// speaker activation is important.</remarks>
        /// <param name="originalState">The original value of the speaker control port. Used to preserve existing port state while enabling the
        /// speaker.</param>

        // Very soft enable/disable to minimize audible clicks
        private static void UltraSoftEnableSpeaker(byte originalState)
        {
            // Configure Timer 2 for square wave mode
            Out32(0x43, 0xB6);
            int div = PIT_BASE_FREQ / ULTRASONIC_FREQ;
            Out32(0x42, (byte)(div & 0xFF));
            Out32(0x42, (byte)(div >> 8));

            Thread.Sleep(10); // To stabilize the timer

            // Open only the gate first (bit 0), keep speaker data off
            Out32(0x61, (byte)(originalState | 0x01)); // Bit 1 = 0
            Thread.Sleep(10);

            // Open speaker data (bit 1) to start sound generation
            Out32(0x61, (byte)(originalState | 0x03)); // Bit 0 ve 1
        }

        /// <summary>
        /// Disables the PC speaker by restoring its control port to the specified original state.
        /// </summary>
        /// <remarks>This method is intended for low-level hardware control and should be used with
        /// caution. It restores the speaker control port in two steps to ensure proper deactivation of the speaker.
        /// Requires appropriate permissions to access hardware ports.</remarks>
        /// <param name="originalState">The original value of the speaker control port to restore. This value should represent the state prior to
        /// any modifications made for speaker output.</param>
        private static void UltraSoftDisableSpeaker(byte originalState)
        {
            // Close speaker data first (bit 1)
            Out32(0x61, (byte)((originalState & 0xFE) | 0x01));
            Thread.Sleep(10);

            // Then close the gate (bit 0)
            Out32(0x61, (byte)(originalState & 0xFC));
            Thread.Sleep(10);
        }

        /// <summary>
        /// Checks whether electrical feedback is present and the gate is responsive on port 0x61.
        /// </summary>
        /// <remarks>This method performs a series of read and write operations on port 0x61 to determine
        /// the presence of electrical feedback and gate responsiveness. It is intended for use with hardware that
        /// supports direct port access. If an error occurs during the check, the method returns false.</remarks>
        /// <returns>true if electrical feedback is detected and the gate responds as expected; otherwise, false.</returns>
        public static bool CheckElectricalFeedbackOnPort()
        {
            try
            {
                byte originalState = (byte)Inp32(0x61);

                // Ultra soft start
                UltraSoftEnableSpeaker(originalState);
                Thread.Sleep(50);

                List<byte> enabledSamples = new List<byte>();
                for (int i = 0; i < 20; i++)
                {
                    enabledSamples.Add((byte)Inp32(0x61));
                    Thread.Sleep(1);
                }
                byte stateEnabled = enabledSamples[enabledSamples.Count / 2];

                // Ultra soft close
                UltraSoftDisableSpeaker(originalState);
                Thread.Sleep(50);

                List<byte> disabledSamples = new List<byte>();
                for (int i = 0; i < 20; i++)
                {
                    disabledSamples.Add((byte)Inp32(0x61));
                    Thread.Sleep(1);
                }
                byte stateDisabled = disabledSamples[disabledSamples.Count / 2];

                // Restore original state
                Out32(0x61, originalState);
                Thread.Sleep(20);

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


        /// <summary>
        /// Determines whether the state of the hardware port at address 0x61 is stable by sampling and analyzing bit 5
        /// transitions.
        /// </summary>
        /// <remarks>This method performs multiple rapid reads of the port at address 0x61 and analyzes
        /// the variability and transitions of bit 5 to assess stability. It is intended for use in scenarios where
        /// reliable detection of port state changes is required. If an error occurs during the operation, the method
        /// returns false.</remarks>
        /// <returns>true if bit 5 of the port state varies and at least three transitions are detected during sampling;
        /// otherwise, false.</returns>
        public static bool CheckPortStateStability()
        {
            try
            {
                byte originalState = (byte)Inp32(0x61);

                // Ultra soft start
                UltraSoftEnableSpeaker(originalState);
                Thread.Sleep(50);

                List<byte> samples = new List<byte>();
                for (int i = 0; i < 100; i++)
                {
                    samples.Add((byte)Inp32(0x61));
                }

                // Ultra soft close
                UltraSoftDisableSpeaker(originalState);
                Out32(0x61, originalState);
                Thread.Sleep(20);

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

                bool sufficientTransitions = bit5Transitions >= 3;

                return bit5Varies && sufficientTransitions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking port state stability: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Performs an advanced frequency sweep test using ultrasonic frequencies to verify speaker output
        /// functionality.
        /// </summary>
        /// <remarks>This test uses frequencies above the audible range to avoid producing audible sounds.
        /// It configures the system timer and speaker control ports to generate and detect output transitions. The
        /// method is intended for diagnostic purposes and may require appropriate hardware access privileges.</remarks>
        /// <returns>true if at least one tested ultrasonic frequency produces detectable output transitions; otherwise, false.</returns>
        public static bool AdvancedFrequencySweepTest()
        {
            try
            {
                Console.WriteLine("\n=== Advanced Frequency Sweep Test (Ultrasonic) ===");
                byte originalState = (byte)Inp32(0x61);
                bool anyFrequencyWorks = false;

                // Higher frequencies to avoid audible range
                int[] testFrequencies = { 30000, 35000, 38000 };

                foreach (int freq in testFrequencies)
                {
                    int div = PIT_BASE_FREQ / freq;
                    if (div < 1) continue;

                    // Configure Timer 2 for square wave mode
                    Out32(0x43, 0xB6);
                    Out32(0x42, (byte)(div & 0xFF));
                    Out32(0x42, (byte)(div >> 8));
                    Thread.Sleep(10);

                    // Open gate first (bit 0), keep speaker data off
                    Out32(0x61, (byte)(originalState | 0x01));
                    Thread.Sleep(10);

                    // Then open speaker data (bit 1)
                    Out32(0x61, (byte)(originalState | 0x03));
                    Thread.Sleep(30);

                    List<byte> samples = new List<byte>();
                    for (int i = 0; i < 50; i++)
                    {
                        samples.Add((byte)Inp32(0x61));
                    }

                    // Close speaker data first
                    Out32(0x61, (byte)((originalState & 0xFE) | 0x01));
                    Thread.Sleep(10);

                    // Close gate
                    Out32(0x61, (byte)(originalState & 0xFC));
                    Thread.Sleep(10);

                    int transitions = 0;
                    for (int i = 0; i < samples.Count - 1; i++)
                    {
                        if ((samples[i] & 0x20) != (samples[i + 1] & 0x20))
                            transitions++;
                    }

                    Console.WriteLine($"  {freq} Hz: {transitions} transitions (SILENT)");

                    if (transitions >= 2)
                    {
                        anyFrequencyWorks = true;
                        break;
                    }
                }

                Out32(0x61, originalState);
                Thread.Sleep(20);
                return anyFrequencyWorks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in frequency sweep test: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Determines whether the system speaker is currently functional based on available hardware diagnostics.
        /// </summary>
        /// <remarks>This method performs multiple hardware checks to assess the functionality of the
        /// system speaker. It is suitable for use in scenarios where reliable audio output is required and hardware
        /// status must be verified before proceeding.</remarks>
        /// <returns>true if at least one diagnostic test indicates the system speaker is operational; otherwise, false.</returns>
        public static bool IsFunctionalSystemSpeaker()
        {
            bool electricalFeedbackValid = CheckElectricalFeedbackOnPort();
            bool portStateStable = CheckPortStateStability();
            bool frequencySweepWorks = AdvancedFrequencySweepTest();

            return electricalFeedbackValid || portStateStable || frequencySweepWorks;
        }

        /// <summary>
        /// Performs a diagnostic test to detect the presence and functionality of the system speaker or buzzer using
        /// silent electrical and frequency-based checks. The results are output to the console.
        /// </summary>
        /// <remarks>This method does not produce audible sounds and is suitable for environments where
        /// silent hardware verification is required. The output includes detailed port state information and possible
        /// reasons for detection failure. This method is intended for use on systems with direct hardware access;
        /// results may vary on virtual machines or systems with restricted port access.</remarks>
        public static void LogSystemSpeakerStatus()
        {
            Console.WriteLine("=== System Speaker Detection Test (ULTRA SILENT MODE) ===\n");
            Console.WriteLine($"Using ultrasonic frequency: {ULTRASONIC_FREQ} Hz with ramped transitions\n");

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

        /// <summary>
        /// Displays detailed diagnostic information about the behavior of the debug port when interacting with
        /// ultrasonic frequencies.
        /// </summary>
        /// <remarks>This method samples the port state multiple times at a specified ultrasonic frequency
        /// and outputs the results to the console for analysis. It is intended for debugging and hardware investigation
        /// scenarios, and does not modify application state beyond console output. Use this method to observe port bit
        /// patterns and speaker control behavior during ultrasonic operations.</remarks>
        public static void DebugPortBehavior()
        {
            Console.WriteLine("\n=== Debug: Detailed Port Behavior (Ultrasonic) ===");

            byte originalState = (byte)Inp32(0x61);
            UltraSoftEnableSpeaker(originalState);
            Thread.Sleep(50);

            Console.WriteLine($"Sampling with {ULTRASONIC_FREQ} Hz (silent frequency):");
            for (int i = 0; i < 50; i++)
            {
                byte sample = (byte)Inp32(0x61);
                Console.WriteLine($"Sample {i:D2}: 0x{sample:X2} | Bit5={((sample & 0x20) != 0 ? "1" : "0")} Bits0-1={sample & 0x03:X}");
            }

            UltraSoftDisableSpeaker(originalState);
            Out32(0x61, originalState);
        }

        /// <summary>
        /// Serves as the entry point for the application.
        /// </summary>
        /// <remarks>This method is called automatically when the program starts. It initializes the
        /// application by invoking startup routines as required.</remarks>
        static void Main()
        {
            LogSystemSpeakerStatus();
            //DebugPortBehavior();
        }
    }
}