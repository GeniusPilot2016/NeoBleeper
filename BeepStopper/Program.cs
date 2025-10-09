using BeepStopper.Properties;
using NeoBleeper;
using System.Management;
using System.Security.Cryptography.X509Certificates;
using static NeoBleeper.Logger;

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
            Logger.Log("Beep stopper is starting...", LogTypes.Info);
            ApplicationConfiguration.Initialize();
            loadSettings();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string query = "SELECT * FROM Win32_PNPEntity Where DeviceID like '%PNP0800%'";
            Logger.Log("Checking for system speaker presence...", LogTypes.Info);
            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(query);
            ManagementObjectCollection number_of_system_speaker_devices = searcher1.Get();
            bool is_system_speaker_present = number_of_system_speaker_devices.Count >= 1;
            ApplicationConfiguration.Initialize();
            if (is_system_speaker_present)
            {
                Logger.Log("System speaker is present. Starting the beep stopper application.", LogTypes.Info);
                Application.Run(new main_window());
            }
            else
            {
                Logger.Log("System speaker is not present. Therefore, stopping the beep is not possible.", LogTypes.Error);
                MessageBox.Show(Resources.SystemSpeakerNotPresentMessage, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private static void loadSettings()
        {
            var synchronizedSettings = SynchronizedSettings.Load();
            UIHelper.setLanguageByName(synchronizedSettings.Language);
            Logger.Log($"Beep stopper is starting with language: {synchronizedSettings.Language}", LogTypes.Info);
        }
    }
}