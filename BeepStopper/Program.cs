using System.Management;

namespace BeepStopper
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new main_window());
            /*// To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string query = "SELECT * FROM Win32_PNPEntity Where DeviceID like '%PNP0800%'";
            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(query);
            ManagementObjectCollection number_of_system_speaker_devices = searcher1.Get();
            bool is_system_speaker_present = number_of_system_speaker_devices.Count >= 1;
            if(is_system_speaker_present)
            {
                
            }
            else
            {
                MessageBox.Show("System speaker is not present. Therefore, stopping the beep is not possible.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }
    }
}