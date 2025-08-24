using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Management;
using static NeoBleeper.Logger;

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
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            // P-Invoke to set the system timer resolution to 1 ms
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            static extern uint timeBeginPeriod(uint uPeriod);
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            static extern uint timeEndPeriod(uint uPeriod);
            timeBeginPeriod(1); // Set the system timer resolution to 1 ms for better timing accuracy
            Logger.Log("NeoBleeper is starting up.", LogTypes.Info);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var dummyWaveOut = RenderBeep.SynthMisc.waveOut; // Dummy initialization to ensure the waveOut is created before any sound operations
            if (Settings1.Default.ClassicBleeperMode)
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
                Logger.Log("Classic Bleeper Mode is enabled. NeoBleeper will run in Classic Bleeper mode.", LogTypes.Info);
            }
            else
            {
                Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
                Logger.Log("Classic Bleeper Mode is disabled. NeoBleeper will run in standard mode.", LogTypes.Info);
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
                Logger.Log("NeoBleeper has detected that your Google Gemini™ API key is corrupted. Please re-enter your Google Gemini™ API key in the settings window.", LogTypes.Error);
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
                    Logger.Log("API key validation successful", LogTypes.Info);
                }
                catch (Exception ex)
                {
                    // If decryption fails, reset the key and show warning
                    Logger.Log("API key validation failed: " + ex.Message, LogTypes.Error);
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
                            Logger.Log("NeoBleeper is exited.", LogTypes.Info);
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
                                    Logger.Log("System speaker output is not present. NeoBleeper will use sound card to create beeps.", LogTypes.Info);
                                    if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                    {
                                        DialogResult result = system_speaker_warning.ShowDialog();
                                        switch (result)
                                        {
                                            case DialogResult.Yes:
                                                Logger.Log("User has chosen to continue with NeoBleeper.", LogTypes.Info);
                                                Application.Run(new main_window());
                                                Logger.Log("NeoBleeper is started.", LogTypes.Info);
                                                break;
                                            case DialogResult.No:
                                                Logger.Log("User has chosen to exit NeoBleeper.", LogTypes.Info);
                                                Application.Exit();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Application.Run(new main_window());
                                        Logger.Log("NeoBleeper is started without system speaker warning.", LogTypes.Info);
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
                                                    Logger.Log("NeoBleeper is started.", LogTypes.Info);
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
                                                    Logger.Log("System speaker output is present, but it is a compact computer. NeoBleeper will use sound card to create beeps to avoid issues with compact computers.", LogTypes.Info);
                                                    if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                                    {
                                                        DialogResult result = compact_computer_warning.ShowDialog();
                                                        switch (result)
                                                        {
                                                            case DialogResult.Yes:
                                                                Logger.Log("User has chosen to continue with NeoBleeper.", LogTypes.Info);
                                                                Application.Run(new main_window());
                                                                Logger.Log("NeoBleeper is started.", LogTypes.Info);
                                                                break;
                                                            case DialogResult.No:
                                                                Logger.Log("User has chosen to exit NeoBleeper.", LogTypes.Info);
                                                                Application.Exit();
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Application.Run(new main_window());
                                                        Logger.Log("NeoBleeper is started without compact computer warning.", LogTypes.Info);
                                                    }
                                                    break;
                                                }
                                            default:
                                                {
                                                    TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                                    TemporarySettings.creating_sounds.permanently_enabled = false;
                                                    TemporarySettings.eligability_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligability_of_create_beep_from_system_speaker.DeviceType.Unknown;
                                                    Logger.Log("System speaker output is present, but it is an unknown type of computer. NeoBleeper will use sound card to create beeps to avoid issues with unknown type of computers.", LogTypes.Info);
                                                    DialogResult result = unknown_type_of_computer_warning.ShowDialog();
                                                    switch (result)
                                                    {
                                                        case DialogResult.Yes:
                                                            Logger.Log("User has chosen to continue with NeoBleeper.", LogTypes.Info);
                                                            Application.Run(new main_window());
                                                            Logger.Log("NeoBleeper is started.", LogTypes.Info);
                                                            break;
                                                        case DialogResult.No:
                                                            Logger.Log("User has chosen to exit NeoBleeper.", LogTypes.Info);
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
                                                    Logger.Log("User has chosen to continue with NeoBleeper.", LogTypes.Info);
                                                    Application.Run(new main_window());
                                                    Logger.Log("NeoBleeper is started.", LogTypes.Info);
                                                    break;
                                                case DialogResult.No:
                                                    Logger.Log("User has chosen to exit NeoBleeper.", LogTypes.Info);
                                                    Application.Exit();
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Application.Run(new main_window());
                                            Logger.Log("NeoBleeper is started without unknown type of computer warning.", LogTypes.Info);
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