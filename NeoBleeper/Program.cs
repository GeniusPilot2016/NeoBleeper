using System.Management;
using System.Diagnostics;

namespace NeoBleeper
{
    internal static class Program
    {
        /// <summary>
        /// 508b2fc7b16f37635377da7bc575a7e0
        ///  The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {
            Debug.WriteLine("\r\n  _   _            ____  _                           \r\n | \\ | |          |  _ \\| |                          \r\n |  \\| | ___  ___ | |_) | | ___  ___ _ __   ___ _ __ \r\n | . ` |/ _ \\/ _ \\|  _ <| |/ _ \\/ _ \\ '_ \\ / _ \\ '__|\r\n | |\\  |  __/ (_) | |_) | |  __/  __/ |_) |  __/ |   \r\n |_| \\_|\\___|\\___/|____/|_|\\___|\\___| .__/ \\___|_|   \r\n                                    | |              \r\n                                    |_|              \r\n");
            Debug.WriteLine("From Something Unreal to Open Sound � Reviving the Legacy, One Note at a Time. \r\n");
            Debug.WriteLine("https://github.com/GeniusPilot2016/NeoBleeper \r\n");
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            if (Settings1.Default.ClassicBleeperMode)
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
                Debug.WriteLine("Classic Bleeper Mode is enabled. NeoBleeper will run in Classic Bleeper mode.");
            }
            else
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
                Debug.WriteLine("Classic Bleeper Mode is disabled. NeoBleeper will run in standard mode.");
            }
                MIDIIOUtils.InitializeMidi();
            neobleeper_init_system_speaker_warning system_speaker_warning = new neobleeper_init_system_speaker_warning();
            neobleeper_init_display_resolution_warning display_resolution_warning = new neobleeper_init_display_resolution_warning();
            neobleeper_init_compact_computer_warning compact_computer_warning = new neobleeper_init_compact_computer_warning();
            neobleeper_init_unknown_type_of_computer_warning unknown_type_of_computer_warning = new neobleeper_init_unknown_type_of_computer_warning();
            try
            {
                EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
            }
            catch (Exception ex)
            {
                Settings1.Default.geminiAPIKey = String.Empty;
                Settings1.Default.Save();
                EncryptionHelper.ChangeKeyAndIV();
                MessageBox.Show("NeoBleeper has detected that your Google Gemini� API key is corrupted. Please re-enter your Google Gemini� API key in the settings window.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("NeoBleeper has detected that your Google Gemini� API key is corrupted. Please re-enter your Google Gemini� API key in the settings window.");
            }
            try
            {
                // Clear any previously shown warnings
                bool apiKeyWarningShown = false;

                // Check if we have an API key to verify
                if (!string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
                {
                    try
                    {
                        // Try to decrypt the API key
                        string apiKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
                        Debug.WriteLine("API key validation successful");
                    }
                    catch (Exception ex)
                    {
                        // If decryption fails, reset the key and show warning
                        Debug.WriteLine("API key validation failed: " + ex.Message);
                        Settings1.Default.geminiAPIKey = String.Empty;
                        Settings1.Default.Save();
                        EncryptionHelper.ChangeKeyAndIV();
                        MessageBox.Show("NeoBleeper has detected that your Google Gemini� API key is corrupted. Please re-enter your Google Gemini� API key in the settings window.",
                            String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        apiKeyWarningShown = true;
                    }
                }
                switch (Screen.PrimaryScreen.Bounds.Width >= 1024 || Screen.PrimaryScreen.Bounds.Height >= 768)
                {
                    case false:
                        {
                            throw new NotSupportedException("Operation is not supported on this platform: NeoBleeper has detected that your computer's screen resolution does not meet its requirements.");
                        }

                    case true:
                        {
                            string query1 = "SELECT * FROM Win32_PNPEntity Where DeviceID like '%PNP0800%'";
                            string query2 = "SELECT * FROM Win32_SystemEnclosure";
                            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(query1);
                            ManagementObjectCollection number_of_system_speaker_devices = searcher1.Get();
                            TemporarySettings.eligability_of_create_beep_from_system_speaker.is_system_speaker_present = number_of_system_speaker_devices.Count >= 1;
                            try
                            {
                                switch (TemporarySettings.eligability_of_create_beep_from_system_speaker.is_system_speaker_present)
                                {
                                    case false:
                                        {
                                            TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                            TemporarySettings.creating_sounds.permanently_enabled = true;
                                            TemporarySettings.creating_sounds.is_system_speaker_muted = true;
                                            throw new IOException("I/O Exception: NeoBleeper Smart System Speaker Sensor has detected that your computer has no system speaker output or non-standard system speaker output.");
                                        }

                                    case true:
                                        int chassis_type;
                                        ManagementObjectSearcher searcher2 = new ManagementObjectSearcher(query2);
                                        foreach (ManagementObject queryObj in searcher2.Get())
                                        {
                                            var chassisTypes = (UInt16[])(queryObj["ChassisTypes"]);
                                            if (chassisTypes != null && chassisTypes.Length > 0)
                                            {
                                                chassis_type = chassisTypes[0];
                                                switch (chassis_type)
                                                {
                                                    case 3:
                                                    case 4:
                                                    case 5:
                                                    case 6:
                                                    case 7:
                                                    case 17:
                                                    case 18:
                                                    case 22:
                                                    case 23:
                                                        {
                                                            TemporarySettings.creating_sounds.create_beep_with_soundcard = false;
                                                            TemporarySettings.eligability_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligability_of_create_beep_from_system_speaker.DeviceType.ModularComputers;
                                                            Application.Run(new main_window());
                                                            Debug.WriteLine("NeoBleeper is started.");
                                                            break;
                                                        }
                                                    case 8:
                                                    case 9:
                                                    case 10:
                                                    case 11:
                                                    case 13:
                                                    case 14:
                                                    case 15:
                                                    case 16:
                                                    case 24:
                                                    case 30:
                                                    case 31:
                                                    case 32:
                                                        {
                                                            TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                                            TemporarySettings.creating_sounds.permanently_enabled = false;
                                                            TemporarySettings.eligability_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligability_of_create_beep_from_system_speaker.DeviceType.CompactComputers;
                                                            DialogResult result = compact_computer_warning.ShowDialog();
                                                            switch (result)
                                                            {
                                                                case DialogResult.Yes:
                                                                    Debug.WriteLine("User has chosen to continue with NeoBleeper.");
                                                                    Application.Run(new main_window());
                                                                    Debug.WriteLine("NeoBleeper is started.");
                                                                    break;
                                                                case DialogResult.No:
                                                                    Debug.WriteLine("User has chosen to exit NeoBleeper.");
                                                                    Application.Exit();
                                                                    break;
                                                            }
                                                            break;
                                                        }
                                                    default:
                                                        {
                                                            TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                                            TemporarySettings.creating_sounds.permanently_enabled = false;
                                                            TemporarySettings.eligability_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligability_of_create_beep_from_system_speaker.DeviceType.Unknown;
                                                            DialogResult result = unknown_type_of_computer_warning.ShowDialog();
                                                            switch (result)
                                                            {
                                                                case DialogResult.Yes:
                                                                    Debug.WriteLine("User has chosen to continue with NeoBleeper.");
                                                                    Application.Run(new main_window());
                                                                    Debug.WriteLine("NeoBleeper is started.");
                                                                    break;
                                                                case DialogResult.No:
                                                                    Debug.WriteLine("User has chosen to exit NeoBleeper.");
                                                                    Application.Exit();
                                                                    break;
                                                            }
                                                            break;
                                                        }

                                                }
                                            }
                                            else
                                            {
                                                TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                                TemporarySettings.creating_sounds.permanently_enabled = false;
                                                TemporarySettings.eligability_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligability_of_create_beep_from_system_speaker.DeviceType.Unknown;
                                                DialogResult result = unknown_type_of_computer_warning.ShowDialog();
                                                switch (result)
                                                {
                                                    case DialogResult.Yes:
                                                        Debug.WriteLine("User has chosen to continue with NeoBleeper.");
                                                        Application.Run(new main_window());
                                                        Debug.WriteLine("NeoBleeper is started.");
                                                        break;
                                                    case DialogResult.No:
                                                        Debug.WriteLine("User has chosen to exit NeoBleeper.");
                                                        Application.Exit();
                                                        break;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                            catch (IOException e)
                            {
                                Debug.WriteLine(e.Message);
                                DialogResult result = system_speaker_warning.ShowDialog();
                                switch (result)
                                {
                                    case DialogResult.Yes:
                                        Debug.WriteLine("User has chosen to continue with NeoBleeper.");
                                        Application.Run(new main_window());
                                        Debug.WriteLine("NeoBleeper is started.");
                                        break;
                                    case DialogResult.No:
                                        Debug.WriteLine("User has chosen to exit NeoBleeper.");
                                        Application.Exit();
                                        break;
                                }

                                break;
                            }
                            break;
                        }
                }       
            }
            catch (NotSupportedException e)
            {
                Debug.WriteLine(e.Message);
                DialogResult result = display_resolution_warning.ShowDialog();
                if(result==DialogResult.Abort)
                {
                    Debug.WriteLine("NeoBleeper is exited.");
                    Application.Exit();
                }
            }
            MIDIIOUtils.DisposeMidiOutput();
        }
    }
}