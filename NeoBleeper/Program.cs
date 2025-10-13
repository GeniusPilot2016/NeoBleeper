// NeoBleeper - AI-enabled tune creation software using the system speaker (aka PC Speaker) on the motherboard
// Copyright (C) 2023 GeniusPilot2016
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.

using NeoBleeper.Properties;
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
        // The core of the NeoBleeper application, which Robbi-985 (aka SomethingUnreal) would be proud of.
        // Powered by nostalgia and a passion for sound, this program brings the classic system speaker beeps back to life with modern enhancements - Thanks Robbi-985 (aka SomethingUnreal)!
        public static string filePath = null;
        public static bool isAnySoundDeviceExist = SoundRenderingEngine.WaveSynthEngine.checkIfAnySoundDeviceExistAndEnabled();
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Length > 0)
            {
                filePath = args[0];
            }
            bool shouldRun = false;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Logger.Log("NeoBleeper is starting up.", LogTypes.Info);
            SoundRenderingEngine.SystemSpeakerBeepEngine.SpecifyStorageType(); // Specify storage type for system speaker beep engine to prevent critical errors in some systems where uses mechanical storage drives
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
            switch (Settings1.Default.ClassicBleeperMode)
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
            try
            {
                var synchronizedSettings = SynchronizedSettings.Load();
                // Load synchronized settings and check for mismatches for beep stopper to apply NeoBleeper's theme and language settings
                if (synchronizedSettings.Language != Settings1.Default.preferredLanguage ||
                    synchronizedSettings.Theme != Settings1.Default.theme)
                {
                    Logger.Log("Settings mismatch detected. Updating synchronized settings.", LogTypes.Warning);
                    synchronizedSettings.Language = Settings1.Default.preferredLanguage;
                    synchronizedSettings.Theme = Settings1.Default.theme;
                    synchronizedSettings.Save();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Failed to load or save synchronized settings for beep stopper: " + ex.Message, LogTypes.Error);
            }
        }
    }
}