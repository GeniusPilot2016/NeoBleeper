using Microsoft.Extensions.Logging;
using NeoBleeper.Properties;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
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

        public static bool isAnySoundDeviceExist = SoundRenderingEngine.WaveSynthEngine.checkIfAnySoundDeviceExistAndEnabled();
        [STAThread]
        static void Main()
        {
            bool shouldRun = false;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Logger.Log("NeoBleeper is starting up.", LogTypes.Info);

            // Configure application before ApplicationConfiguration.Initialize()
            ConfigureApplication();

            // Initialize application configuration
            ApplicationConfiguration.Initialize();

            // Initialize audio after application configuration
            var dummyWaveOut = SoundRenderingEngine.WaveSynthEngine.waveOut; // Dummy initialization to ensure the waveOut is created before any sound operations

            MIDIIOUtils.InitializeMidi();

            // Check if it has an API key to verify
            if (!string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
            {
                try
                {
                    string apiKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
                    Logger.Log("API key validation successful", LogTypes.Info);
                }
                catch (CryptographicException ex)
                {
                    Logger.Log($"API key validation failed: {ex.Message}\nThis may be due to a corrupted API key or a change in encryption keys. The API key has been reset.", LogTypes.Error);
                    if (!string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
                    {
                        Settings1.Default.geminiAPIKey = String.Empty;
                        Settings1.Default.Save(); // Save the settings after clearing the API key
                        EncryptionHelper.ChangeKeyAndIV();
                    }
                    MessageBox.Show(Resources.MessageAPIKeyIsCorrupted,
                        String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            switch (GetInformations.isResolutionSupported())
            {
                case false:
                    {
                        ShowCentralizedWarning(WarningType.DisplayResolution);
                        break;
                    }

                case true:
                    {
                        TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_system_speaker_present = SoundRenderingEngine.SystemSpeakerBeepEngine.isSystemSpeakerExist();
                        switch (TemporarySettings.eligibility_of_create_beep_from_system_speaker.is_system_speaker_present)
                        {
                            case false:
                                {
                                    TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                    TemporarySettings.creating_sounds.permanently_enabled = true;
                                    Logger.Log("System speaker output is not present. NeoBleeper will use sound card to create beeps.", LogTypes.Info);
                                    if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                    {
                                        shouldRun = ShowCentralizedWarning(WarningType.SystemSpeaker);
                                    }
                                    else
                                    {
                                        shouldRun = true;
                                        Logger.Log("NeoBleeper is starting without system speaker warning.", LogTypes.Info);
                                    }
                                    break;
                                }
                            case true:
                                switch (GetInformations.getTypeOfComputer())
                                {
                                    case GetInformations.computerTypes.ModularComputer:
                                        {
                                            TemporarySettings.creating_sounds.create_beep_with_soundcard = false;
                                            TemporarySettings.eligibility_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligibility_of_create_beep_from_system_speaker.DeviceType.ModularComputers;
                                            shouldRun = true;
                                            break;
                                        }
                                    case GetInformations.computerTypes.CompactComputer:
                                        {
                                            TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                            TemporarySettings.creating_sounds.permanently_enabled = false;
                                            TemporarySettings.eligibility_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligibility_of_create_beep_from_system_speaker.DeviceType.CompactComputers;
                                            Logger.Log("System speaker output is present, but it is a compact computer. NeoBleeper will use sound card to create beeps to avoid issues with compact computers.", LogTypes.Info);
                                            if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                            {
                                                shouldRun = ShowCentralizedWarning(WarningType.CompactComputer);
                                            }
                                            else
                                            {
                                                shouldRun = true;
                                                Logger.Log("NeoBleeper is starting without compact computer warning.", LogTypes.Info);
                                            }
                                            break;
                                        }
                                    default:
                                        {
                                            TemporarySettings.creating_sounds.create_beep_with_soundcard = true;
                                            TemporarySettings.creating_sounds.permanently_enabled = false;
                                            TemporarySettings.eligibility_of_create_beep_from_system_speaker.deviceType = TemporarySettings.eligibility_of_create_beep_from_system_speaker.DeviceType.Unknown;
                                            Logger.Log("System speaker output is present, but it is an unknown type of computer. NeoBleeper will use sound card to create beeps to avoid issues with unknown type of computers.", LogTypes.Info);
                                            if (!Settings1.Default.dont_show_system_speaker_warnings_again)
                                            {
                                                shouldRun = ShowCentralizedWarning(WarningType.UnknownComputer);
                                            }
                                            else
                                            {
                                                shouldRun = true;
                                                Logger.Log("NeoBleeper is starting without compact computer warning.", LogTypes.Info);
                                            }
                                            break;
                                        }

                                }
                                break;
                        }
                        break;
                    }
            }

            if (shouldRun)
            {
                try
                {
                    Application.Run(new main_window());
                }
                catch (Exception ex)
                {
                    Logger.Log("An error occurred while running the application: " + ex.Message, LogTypes.Error);
                    MessageBox.Show(Resources.AnErrorOccurredWhileRunningApplication + ex.Message, Resources.TextError, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Logger.Log("NeoBleeper is exited.", LogTypes.Info); // Exit when both normal exit and fatal error
            }
            else
            {
                Logger.Log("NeoBleeper is exited.", LogTypes.Info); // Exit due to user choice or resolution issue
            }
            MIDIIOUtils.DisposeMidiOutput();
        }
        enum WarningType
        {
            DisplayResolution,
            SystemSpeaker,
            CompactComputer,
            UnknownComputer
        }

        static bool ShowCentralizedWarning(WarningType type)
        {
            Form warningForm = null;
            switch (type)
            {
                case WarningType.DisplayResolution:
                    warningForm = new neobleeper_init_display_resolution_warning();
                    break;
                case WarningType.SystemSpeaker:
                    warningForm = new neobleeper_init_system_speaker_warning();
                    break;
                case WarningType.CompactComputer:
                    warningForm = new neobleeper_init_compact_computer_warning();
                    break;
                case WarningType.UnknownComputer:
                    warningForm = new neobleeper_init_unknown_type_of_computer_warning();
                    break;
            }
            if (warningForm != null)
            {
                return ShowWarningAndGetUserDecision(warningForm);
            }
            return true;
        }
        private static bool ShowWarningAndGetUserDecision(Form warningForm)
        {
            DialogResult result = warningForm.ShowDialog();
            if (result == DialogResult.Yes)
            {
                Logger.Log("User has chosen to continue with NeoBleeper.", LogTypes.Info);
                return true;
            }
            else if (result == DialogResult.No)
            {
                Logger.Log("User has chosen to exit NeoBleeper.", LogTypes.Info);
                return false;
            }
                return false;
        }
        private static void ConfigureApplication()
        {
            switch(Settings1.Default.ClassicBleeperMode)
            {
                case true:
                    Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
                    Logger.Log("Classic Bleeper Mode is enabled. NeoBleeper will run in Classic Bleeper mode.", LogTypes.Info);
                    break;
                case false:
                    Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
                    Logger.Log("Classic Bleeper Mode is disabled. NeoBleeper will run in standard mode.", LogTypes.Info);
                    break;
            }
            UIHelper.setLanguageByName(Settings1.Default.preferredLanguage); // Set the language based on user preference
            Logger.Log($"NeoBleeper is starting with language: {Settings1.Default.preferredLanguage}", LogTypes.Info);
        }
    }
}