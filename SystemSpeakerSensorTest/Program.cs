using System.Runtime.InteropServices;

namespace SystemSpeakerSensorTest
{
    internal class Program
    {
        [DllImport("inpoutx64.dll")]
        extern static void Out32(short PortAddress, short Data);
        [DllImport("inpoutx64.dll")]
        extern static char Inp32(short PortAddress);
        public static bool CheckElectricalFeedbackOnPort()
        {
            try
            {
                byte originalState = (byte)Inp32(0x61);

                // Check the response of bits 0 and 1 (speaker gate control)
                Out32(0x61, (byte)(originalState | 0x03));
                Thread.Sleep(5); // Wait a short time to respond
                byte stateEnabled = (byte)Inp32(0x61);

                Out32(0x61, (byte)(originalState & 0xFC));
                Thread.Sleep(5);
                byte stateDisabled = (byte)Inp32(0x61);

                // Restore the original state
                Out32(0x61, originalState);

                // Check bit 5 (timer 2 output) changes with speaker enabled/disabled
                bool feedbackPresent = ((stateEnabled & 0x20) != (stateDisabled & 0x20));
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
                // Start the PIT in test mode with a known frequency
                Out32(0x43, 0xB6);
                int testFreq = 1000;
                int div = 0x1234dc / testFreq;
                Out32(0x42, (byte)(div & 0xFF));
                Out32(0x42, (byte)(div >> 8));

                byte originalState = (byte)Inp32(0x61);

                // Open the system speaker (aka PC speaker) gate
                Out32(0x61, (byte)(originalState | 0x03));
                Thread.Sleep(20); // Yeterli süre bekle

                // Bit 4 of 0x61 (timer 2 gate) is always changing
                // Bit 5 should change if the speaker is functional
                byte read1 = (byte)Inp32(0x61);
                Thread.Sleep(10);
                byte read2 = (byte)Inp32(0x61);
                Thread.Sleep(10);
                byte read3 = (byte)Inp32(0x61);

                // Restore the original state
                Out32(0x61, originalState);

                // Variability of bit 5 (timer 2 output) indicates a functional system speaker
                bool bit5Changed = ((read1 & 0x20) != (read2 & 0x20)) ||
                                   ((read2 & 0x20) != (read3 & 0x20));

                // Checkability of control bits (0 and 1) indicates instability
                bool controlBitsStable = (read1 & 0x03) == (read2 & 0x03) &&
                                          (read2 & 0x03) == (read3 & 0x03);

                return bit5Changed && controlBitsStable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking port state stability: {ex.Message}");
                return false;
            }
        }
        public static bool IsFunctionalSystemSpeaker()
        {
            bool electricalFeedbackValid = CheckElectricalFeedbackOnPort();
            bool portStateStable = CheckPortStateStability();

            return electricalFeedbackValid && portStateStable;
        }
        public static void LogSystemSpeakerStatus()
        {
            bool electricalFeedbackValid = CheckElectricalFeedbackOnPort();
            Console.WriteLine($"Electrical feedback valid: {electricalFeedbackValid}");

            bool portStateStable = CheckPortStateStability();
            Console.WriteLine($"Port state stable: {portStateStable}");

            if (electricalFeedbackValid && portStateStable)
            {
                Console.WriteLine("Functional system speaker or buzzer detected.");
            }
            else
            {
                Console.WriteLine("No functional system speaker or buzzer detected.");
            }
        }
        static void Main()
        {
            LogSystemSpeakerStatus();
        }
    }
}