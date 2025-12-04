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
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using static NeoBleeper.Logger;
using static UIHelper;

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
        public static bool isAnySoundDeviceExist = SoundRenderingEngine.WaveSynthEngine.CheckIfAnySoundDeviceExistAndEnabled();
        public static SplashScreen splashScreen = new SplashScreen();
        public static bool isAffectedChipsetChecked = false; // Flag to indicate if the affected chipset has been checked
        public static bool isExistenceOfSystemSpeakerChecked = false; // Flag to indicate if the existence of system speaker has been checked
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    filePath = args[0];
                }
                // Initialize application configuration
                ApplicationConfiguration.Initialize();
                LoadSettingsIfNeeded(); // Load settings if needed (upgrade from previous versions)
                ConfigureApplication();
                splashScreen.Show();
                CheckAndPlaceInpOutX64(); // Check presence of InpOutx64.dll and place it if not present
                if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                {
                    // Skip system speaker detection on ARM64 architecture such as most of Copilot+ devices due to lack of system speaker support
                    TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isChipsetAffectedFromSystemSpeakerIssues = SoundRenderingEngine.SystemSpeakerBeepEngine.CheckIfChipsetAffectedFromSystemSpeakerIssues();
                    SoundRenderingEngine.SystemSpeakerBeepEngine.AwakeSystemSpeakerIfNeeded();
                    TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isSystemSpeakerPresent = SoundRenderingEngine.SystemSpeakerBeepEngine.IsSystemSpeakerExist();
                    SoundRenderingEngine.SystemSpeakerBeepEngine.SpecifyStorageType(); // Specify storage type for system speaker beep engine to prevent critical errors in some systems where uses mechanical storage drives
                }
                SynchronizeSettings();
                bool shouldRun = false;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                Logger.Log("NeoBleeper is starting up.", LogTypes.Info);
                SetStatusForClassicBleeperModeAndLanguage();
                // Initialize audio after application configuration
                var dummyWaveOut = SoundRenderingEngine.WaveSynthEngine.waveOut; // Dummy initialization to ensure the waveOut is created before any sound operations
                Logger.Log("MIDI input/output is being initialized...", LogTypes.Info);
                splashScreen.UpdateStatus(Resources.StatusMIDIIOInitializing);
                MIDIIOUtils.InitializeMidi();
                Logger.Log("MIDI input/output initialization completed.", LogTypes.Info);
                splashScreen.UpdateStatus(Resources.StatusMIDIIOInitializationCompleted, 5);

                // Check if it has an API key to verify
                if (!string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
                {
                    try
                    {
                        splashScreen.UpdateStatus(Resources.StatusAPIKeyValidating);
                        string apiKey = EncryptionHelper.DecryptString(Settings1.Default.geminiAPIKey);
                        Logger.Log("API key validation successful", LogTypes.Info);
                        splashScreen.UpdateStatus(Resources.StatusAPIKeyValidationSuccessful, 5);
                    }
                    catch (CryptographicException ex)
                    {
                        Logger.Log($"API key validation failed: {ex.Message}\nThis may be due to a corrupted API key or a change in encryption keys. The API key has been reset.", LogTypes.Error);
                        splashScreen.UpdateStatus(Resources.MessageAPIKeyIsCorrupted);
                        if (!string.IsNullOrEmpty(Settings1.Default.geminiAPIKey))
                        {
                            Settings1.Default.geminiAPIKey = String.Empty;
                            Settings1.Default.Save(); // Save the settings after clearing the API key
                            EncryptionHelper.ChangeKeyAndIV();
                        }
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(Settings1.Default.geminiAPIKey) && !Settings1.Default.googleGeminiTermsOfServiceAccepted)
                        {
                            Settings1.Default.googleGeminiTermsOfServiceAccepted = true; // Assume accepted if API key is valid but Terms of Service acceptance is not recorded in older versions that did not have this setting
                        }
                    }
                }
                splashScreen.UpdateStatus(Resources.StatusDisplayResolutionCheck);
                switch (GetInformations.IsResolutionSupported())
                {
                    case false:
                        {
                            Logger.Log("Display resolution is not supported. NeoBleeper requires a minimum resolution of 1024x768 to run properly.", LogTypes.Error);
                            splashScreen.UpdateStatus(Resources.StatusDisplayResolutionNotSupported);
                            ShowCentralizedWarning(WarningType.DisplayResolution);
                            break;
                        }

                    case true:
                        {
                            Logger.Log("Display resolution is supported.", LogTypes.Info);
                            splashScreen.UpdateStatus(Resources.StatusDisplayResolutionIsSupported, 10);
                            if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
                            {
                                // Open main form without any warnings on ARM64 architecture such as most of Copilot+ devices due to lack of system speaker support
                                switch (TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isSystemSpeakerPresent)
                                {
                                    case false:
                                        {
                                            TemporarySettings.CreatingSounds.createBeepWithSoundDevice = true;
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
                                        splashScreen.UpdateStatus(Resources.StatusComputerTypeDetecting);
                                        switch (GetInformations.GetTypeOfComputer())
                                        {
                                            case GetInformations.computerTypes.ModularComputer:
                                                {
                                                    TemporarySettings.CreatingSounds.createBeepWithSoundDevice = false || TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isChipsetAffectedFromSystemSpeakerIssues;
                                                    TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.deviceType = TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.DeviceType.ModularComputers;
                                                    shouldRun = true;
                                                    if (TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isChipsetAffectedFromSystemSpeakerIssues)
                                                    {
                                                        Logger.Log("System speaker output is present, but the chipset is known to have issues with system speaker. NeoBleeper will use sound card to create beeps to avoid issues.", LogTypes.Info);
                                                    }
                                                    else
                                                    {
                                                        Logger.Log("System speaker output is present and the chipset is not known to have issues with system speaker. NeoBleeper will use system speaker to create beeps.", LogTypes.Info);
                                                    }
                                                    splashScreen.UpdateStatus(Resources.StatusModularComputerDetected, 5);
                                                    break;
                                                }
                                            case GetInformations.computerTypes.CompactComputer:
                                                {
                                                    TemporarySettings.CreatingSounds.createBeepWithSoundDevice = true || TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isChipsetAffectedFromSystemSpeakerIssues;
                                                    TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.deviceType = TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.DeviceType.CompactComputers;
                                                    Logger.Log("System speaker output is present, but it is a compact computer. NeoBleeper will use sound card to create beeps to avoid issues with compact computers.", LogTypes.Info);
                                                    splashScreen.UpdateStatus(Resources.StatusCompactComputerDetected, 5);
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
                                                    TemporarySettings.CreatingSounds.createBeepWithSoundDevice = true || TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.isChipsetAffectedFromSystemSpeakerIssues; ;
                                                    TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.deviceType = TemporarySettings.EligibilityOfCreateBeepFromSystemSpeaker.DeviceType.Unknown;
                                                    Logger.Log("System speaker output is present, but it is an unknown type of computer. NeoBleeper will use sound card to create beeps to avoid issues with unknown type of computers.", LogTypes.Info);
                                                    splashScreen.UpdateStatus(Resources.StatusUnknownComputerTypeDetected, 10);
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
                            }
                            else
                            {
                                TemporarySettings.CreatingSounds.createBeepWithSoundDevice = true;
                                Logger.Log("Running on ARM64 architecture. NeoBleeper will use sound device to create beeps.", LogTypes.Info);
                                shouldRun = true;
                            }
                            break;
                        }
                }

                if (shouldRun)
                {
                    splashScreen.UpdateStatus(Resources.StatusInitializationCompleted, 0, true);
                    splashScreen.ResponsiveWait(2000);
                    splashScreen.Close();
                    Application.Run(new MainWindow());
                }
            }
            catch (Exception ex)
            {
                splashScreen.Close();
                Logger.Log("An error occurred while running the application: " + ex.Message, LogTypes.Error);
                CrashReportingForm.GenerateAndShowCrashReport(ex.GetType().ToString(), ex.Message, ex.StackTrace);
                Application.Exit();
            }
            finally
            {
                UninitializeMIDI();
                Logger.Log("NeoBleeper is exited.", LogTypes.Info); // Exit from application when main form is closed, on error or user chooses to exit from warning
            }
        }
        public static void UninitializeExtendedEvents()
        {
            Logger.Log("Uninitializing extended event managers...", LogTypes.Info);
            ThemeManager.Cleanup();
            PowerManager.Cleanup();
            InputLanguageManager.Cleanup();
            Logger.Log("Extended event managers uninitialization completed.", LogTypes.Info);
        }
        public static void UninitializeMIDI()
        {
            Logger.Log("Uninitializing MIDI input/output...", LogTypes.Info);
            MIDIIOUtils.DisposeMidiOutput();
            Logger.Log("MIDI input/output uninitialization completed.", LogTypes.Info);
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
                    warningForm = new InitDisplayResolutionWarning();
                    break;
                case WarningType.SystemSpeaker:
                    warningForm = new InitSystemSpeakerWarning();
                    break;
                case WarningType.CompactComputer:
                    warningForm = new InitCompactComputerWarning();
                    break;
                case WarningType.UnknownComputer:
                    warningForm = new InitUnknownTypeOfComputerWarning();
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
            splashScreen.UpdateStatus(Resources.StatusInitializationCompleted, 0, true);
            splashScreen.Close();
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
            PowerManager.Initialize();
            ThemeManager.Initialize();
            InputLanguageManager.Initialize();
            switch (Settings1.Default.ClassicBleeperMode)
            {
                case true:
                    Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NonClientAreaEnabled;
                    break;
                case false:
                    Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
                    break;
            }
            UIHelper.SetLanguageByName(Settings1.Default.preferredLanguage); // Set the language based on user preference
        }
        private static void SynchronizeSettings()
        {
            // Synchronize settings with beep stopper if not running on ARM64 architecture for sync Beep Stopper settings (language and theme)
            if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
            {
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
        private static void SetStatusForClassicBleeperModeAndLanguage()
        {
            switch (Settings1.Default.ClassicBleeperMode)
            {
                case true:
                    Logger.Log("Classic Bleeper Mode is enabled. NeoBleeper will run in Classic Bleeper mode.", LogTypes.Info);
                    splashScreen.UpdateStatus(Resources.StatusClassicBleeperModeEnabled, 10);
                    break;
                case false:
                    Logger.Log("Classic Bleeper Mode is disabled. NeoBleeper will run in standard mode.", LogTypes.Info);
                    splashScreen.UpdateStatus(Resources.StatusClassicBleeperModeDisabled, 10);
                    break;
            }
            Logger.Log($"NeoBleeper is starting with language: {Settings1.Default.preferredLanguage}", LogTypes.Info);
            splashScreen.UpdateStatus(Resources.StatusProgramLanguage + Settings1.Default.preferredLanguage, 10);
        }
        private static void CheckAndPlaceInpOutX64()
        {
            if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64) // Skip InpOutx64.dll placement on ARM64 architecture such as most of Copilot+ devices due to lack of system speaker support
            {
                var inpOutX64File = Resources.inpoutx64; // InpOutx64.dll binary resource
                var inpOutX64Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InpOutx64.dll");
                splashScreen.UpdateStatus(Resources.StatusCheckingInpOutX64PresenceAndIntegrity, 5);
                if (!IsInpOutX64PresentAndValid()) // Check if InpOutx64.dll is present and valid
                                                   // If not present or broken, place the DLL file
                {
                    try
                    {
                        splashScreen.UpdateStatus(Resources.StatusInpOutX64IsMissingOrCorrupted);
                        File.WriteAllBytes(inpOutX64Path, inpOutX64File);
                        splashScreen.UpdateStatus(Resources.StatusInpOutX64Placed, 5);
                    }
                    catch (Exception ex)
                    {
                        splashScreen.UpdateStatus(Resources.InpOutX64PlaceError + ex.Message);
                    }
                }
                else
                {
                    splashScreen.UpdateStatus(Resources.StatusInpOutX64IsPresentAndValid);
                }
            }
        }
        private static bool IsInpOutX64PresentAndValid() // Check if InpOutx64.dll is present and valid by comparing SHA256 hash
        {
            var inpOutX64Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InpOutx64.dll");
            if (File.Exists(inpOutX64Path)) // Check if file exists
            {
                string expectedSHA256Hash = string.Empty;
                string actualSHA256Hash = string.Empty;
                using (SHA256 sha256 = SHA256.Create()) // Create SHA256 instance to compute hash
                {
                    byte[] hashBytes = sha256.ComputeHash(Resources.inpoutx64);
                    expectedSHA256Hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    using (FileStream fs = File.OpenRead(inpOutX64Path))
                    {
                        byte[] existingHashBytes = sha256.ComputeHash(fs);
                        actualSHA256Hash = BitConverter.ToString(existingHashBytes).Replace("-", "").ToLowerInvariant();
                    }
                    return expectedSHA256Hash == actualSHA256Hash; // Compare hashes and return result
                }
            }
            return false; // File does not exist
        }
        private static void LoadSettingsIfNeeded()
        {
            if (!Settings1.Default.HasSettingsUpgraded)
            {
                Settings1.Default.Upgrade();
                Settings1.Default.HasSettingsUpgraded = true;
                Settings1.Default.Save();
            }
        }
    }
}