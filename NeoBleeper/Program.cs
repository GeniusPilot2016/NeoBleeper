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
        public static bool isAnySoundDeviceExist = RenderBeep.SynthMisc.checkIfAnySoundDeviceExistAndEnabled();
        [STAThread]
        static void Main()
        {
            // P-Invoke to set the system timer resolution to 1 ms
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            static extern uint timeBeginPeriod(uint uPeriod);
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            static extern uint timeEndPeriod(uint uPeriod);
            timeBeginPeriod(1); // Set the system timer resolution to 1 ms for better timing accuracy
            Debug.WriteLine("\r\n  _   _            ____  _                           \r\n | \\ | |          |  _ \\| |                          \r\n |  \\| | ___  ___ | |_) | | ___  ___ _ __   ___ _ __ \r\n | . ` |/ _ \\/ _ \\|  _ <| |/ _ \\/ _ \\ '_ \\ / _ \\ '__|\r\n | |\\  |  __/ (_) | |_) | |  __/  __/ |_) |  __/ |   \r\n |_| \\_|\\___|\\___/|____/|_|\\___|\\___| .__/ \\___|_|   \r\n                                    | |              \r\n                                    |_|              \r\n");
            Debug.WriteLine("From Something Unreal to Open Sound – Reviving the Legacy, One Note at a Time. \r\n");
            Debug.WriteLine("https://github.com/GeniusPilot2016/NeoBleeper \r\n");
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var dummyWaveOut = RenderBeep.SynthMisc.waveOut; // Dummy initialization to ensure the waveOut is created before any sound operations
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
                MessageBox.Show("NeoBleeper has detected that your Google Gemini™ API key is corrupted. Please re-enter your Google Gemini™ API key in the settings window.", String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("NeoBleeper has detected that your Google Gemini™ API key is corrupted. Please re-enter your Google Gemini™ API key in the settings window.");
            }
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
                    MessageBox.Show("NeoBleeper has detected that your Google Gemini™ API key is corrupted. Please re-enter your Google Gemini™ API key in the settings window.",
                        String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    apiKeyWarningShown = true;
                }
            }
            switch (Screen.PrimaryScreen.Bounds.Width >= 1024 || Screen.PrimaryScreen.Bounds.Height >= 768)
            {
                case false:
                    {
                        DialogResult result = display_resolution_warning.ShowDialog();
                        if (result == DialogResult.Abort)
                        {
                            Debug.WriteLine("NeoBleeper is exited.");
                            Application.Exit();
                        }
                        break;
                    }

                case true:
                    {
                        string query = "SELECT * FROM Win32_SystemEnclosure";
                        TemporarySettings.eligability_of_create_beep_from_system_speaker.is_system_speaker_present = RenderBeep.BeepClass.isSystemSpeakerExist();
                        switch (TemporarySettings.eligability_of_create_beep_from_system_speaker.is_system_speaker_present)
                        {
                            case false:
                                {
                                    TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                    TemporarySettings.creating_sounds.permanently_enabled = true;
                                    Debug.WriteLine("System speaker output is not present. NeoBleeper will use sound card to create beeps.");
                                    if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                    {
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
                                    }
                                    else
                                    {
                                        Application.Run(new main_window());
                                        Debug.WriteLine("NeoBleeper is started without system speaker warning.");
                                    }
                                    break;
                                }
                            case true:
                                int chassis_type;
                                ManagementObjectSearcher searcher2 = new ManagementObjectSearcher(query);
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
                                                    Debug.WriteLine("System speaker output is present, but it is a compact computer. NeoBleeper will use sound card to create beeps to avoid issues with compact computers.");
                                                    if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                                    {
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
                                                    }
                                                    else
                                                    {
                                                        Application.Run(new main_window());
                                                        Debug.WriteLine("NeoBleeper is started without compact computer warning.");
                                                    }
                                                    break;
                                                }
                                            default:
                                                {
                                                    TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                                    TemporarySettings.creating_sounds.permanently_enabled = false;
                                                    TemporarySettings.eligability_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligability_of_create_beep_from_system_speaker.DeviceType.Unknown;
                                                    Debug.WriteLine("System speaker output is present, but it is an unknown type of computer. NeoBleeper will use sound card to create beeps to avoid issues with unknown type of computers.");
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
                                        if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                        {
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
                                        else
                                        {
                                            Application.Run(new main_window());
                                            Debug.WriteLine("NeoBleeper is started without unknown type of computer warning.");
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    }
            }
            timeEndPeriod(1); // Reset the system timer resolution to default
            MIDIIOUtils.DisposeMidiOutput();
        }
    }
}