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
        // 508b2fc7b16f37635377da7bc575a7e0
        // The core of the NeoBleeper application, which Robbi-985 (aka SomethingUnreal) would be proud of.
        // Powered by nostalgia and a passion for sound, this program brings the classic system speaker beeps back to life with modern enhancements - Thanks Robbi-985 (aka SomethingUnreal)!
        public static string filePath = null;
        public static bool isAnySoundDeviceExist = SoundRenderingEngine.WaveSynthEngine.CheckIfAnySoundDeviceExistAndEnabled();
        public static SplashScreen splashScreen = new SplashScreen();
        public static bool isAffectedChipsetChecked = false; // Flag to indicate if the affected chipset has been checked
        public static bool isExistenceOfSystemSpeakerChecked = false; // Flag to indicate if the existence of system speaker has been checked
        public static bool isFirstRun = false; // Flag to indicate if it's the first run of the application
        /// <summary>
        /// Serves as the main entry point for the NeoBleeper application.
        /// </summary>
        /// <remarks>This method initializes application configuration, performs hardware and environment
        /// checks, validates user settings, and starts the main application window. It also handles application startup
        /// errors and ensures proper cleanup on exit. The method must be called with the [STAThread] attribute, as
        /// required for Windows Forms applications.</remarks>
        /// <param name="args">An array of command-line arguments supplied to the application. The first argument, if provided, is used as
        /// the file path to open at startup.</param>
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

        /// <summary>
        /// Releases resources and performs cleanup for all extended event managers.
        /// </summary>
        /// <remarks>Call this method to uninitialize event managers related to themes, power management,
        /// and input language handling. After calling this method, extended event functionality may no longer be
        /// available until reinitialized.</remarks>
        public static void UninitializeExtendedEvents()
        {
            Logger.Log("Uninitializing extended event managers...", LogTypes.Info);
            ThemeManager.Cleanup();
            PowerManager.Cleanup();
            InputLanguageManager.Cleanup();
            Logger.Log("Extended event managers uninitialization completed.", LogTypes.Info);
        }

        /// <summary>
        /// Releases resources and shuts down MIDI input and output subsystems.
        /// </summary>
        /// <remarks>Call this method to cleanly uninitialize MIDI functionality when it is no longer
        /// needed. After calling this method, MIDI input and output operations will no longer be available until
        /// reinitialized.</remarks>
        public static void UninitializeMIDI()
        {
            Logger.Log("Uninitializing MIDI input/output...", LogTypes.Info);
            MIDIIOUtils.DisposeMidiOutput();
            Logger.Log("MIDI input/output uninitialization completed.", LogTypes.Info);
        }

        /// <summary>
        /// Specifies the type of warning that can be reported by the system.
        /// </summary>
        enum WarningType
        {
            DisplayResolution,
            SystemSpeaker,
            CompactComputer,
            UnknownComputer
        }

        /// <summary>
        /// Displays a centralized warning dialog corresponding to the specified warning type and returns the user's
        /// decision.
        /// </summary>
        /// <remarks>Use this method to present standardized warning dialogs for various system
        /// conditions. The method blocks until the user responds to the dialog. If an unrecognized warning type is
        /// provided, no dialog is shown and the method returns true.</remarks>
        /// <param name="type">The type of warning to display. Determines which warning dialog is shown to the user.</param>
        /// <returns>true if the user chooses to proceed or if no warning is shown; otherwise, false.</returns>
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

        /// <summary>
        /// Displays the specified warning form as a modal dialog and returns the user's decision to continue or exit.
        /// </summary>
        /// <remarks>The method logs the user's decision and closes any active splash screen before
        /// displaying the warning form. If the dialog result is neither Yes nor No, the method returns false.</remarks>
        /// <param name="warningForm">The warning form to display to the user. Must not be null. The form should return a DialogResult of Yes or
        /// No to indicate the user's choice.</param>
        /// <returns>true if the user chooses to continue; otherwise, false.</returns>
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

        /// <summary>
        /// Configures core application services and applies user interface settings at startup.
        /// </summary>
        /// <remarks>This method initializes essential managers and sets application-wide options such as
        /// power management, theme, input language, and visual style state. It should be called once during application
        /// startup before displaying any user interface elements to ensure consistent behavior and
        /// appearance.</remarks>
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

        /// <summary>
        /// Synchronizes the language and theme settings with the beep stopper component if the application is not
        /// running on ARM64 architecture.
        /// </summary>
        /// <remarks>This method ensures that the beep stopper's settings for language and theme match
        /// those of the main application. Synchronization is skipped on ARM64 platforms, as the beep stopper is not
        /// supported on that architecture. If a mismatch is detected, the beep stopper's settings are updated to
        /// reflect the current application preferences.</remarks>
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

        /// <summary>
        /// Updates the splash screen and logs status messages to reflect the current Classic Bleeper mode and preferred
        /// language settings during application startup.
        /// </summary>
        /// <remarks>This method should be called during the application's initialization phase to ensure
        /// that the user is informed of the selected mode and language. The status messages are displayed on the splash
        /// screen and written to the application log for diagnostic purposes.</remarks>
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

        /// <summary>
        /// Checks for the presence and integrity of the InpOutx64.dll file and places it in the application directory
        /// if necessary.
        /// </summary>
        /// <remarks>This method skips placement of InpOutx64.dll on ARM64 architectures, as system
        /// speaker support is not available on most ARM64 devices. The method updates the splash screen with status
        /// messages to inform the user of progress and any errors encountered during the process.</remarks>
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

        /// <summary>
        /// Determines whether the InpOutx64.dll file is present in the application's base directory and matches the
        /// expected SHA256 hash.
        /// </summary>
        /// <remarks>This method verifies both the presence and integrity of the InpOutx64.dll file by
        /// comparing its SHA256 hash to the expected value. Use this check to ensure that the DLL has not been tampered
        /// with or replaced.</remarks>
        /// <returns>true if InpOutx64.dll exists and its SHA256 hash matches the expected value; otherwise, false.</returns>
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

        /// <summary>
        /// Sets the application's preferred language based on the operating system's user interface language if no
        /// saved language setting is found.
        /// </summary>
        /// <remarks>If the operating system's language is not among the supported languages, the
        /// preferred language is set to English by default. This method should be called before loading user interface
        /// resources that depend on the application's language setting.</remarks>
        private static void SetLanguageBasedOnOSLanguage() // Set application language based on OS language if no saved settings is found
        {
            var osLanguage = System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            switch (osLanguage)
            {
                case "en":
                    Settings1.Default.preferredLanguage = "English";
                    break;
                case "fr":
                    Settings1.Default.preferredLanguage = "Français";
                    break;
                case "de":
                    Settings1.Default.preferredLanguage = "Deutsch";
                    break;
                case "es":
                    Settings1.Default.preferredLanguage = "Español";
                    break;
                case "ru":
                    Settings1.Default.preferredLanguage = "Русский";
                    break;
                case "uk":
                    Settings1.Default.preferredLanguage = "українська";
                    break;
                case "vi":
                    Settings1.Default.preferredLanguage = "Tiếng Việt";
                    break;
                case "tr":
                    Settings1.Default.preferredLanguage = "Türkçe";
                    break;
                case "it":
                    Settings1.Default.preferredLanguage = "Italiano";
                    break;
                default:
                    Settings1.Default.preferredLanguage = "English"; // Default to English if OS language is not supported
                    break;
            }
            Settings1.Default.Save();
        }

        /// <summary>
        /// Ensures that application settings are loaded and upgraded if necessary.
        /// </summary>
        /// <remarks>This method checks whether the application settings have been upgraded from a
        /// previous version. If not, it performs the upgrade and saves the updated settings. If no previous settings
        /// are found, it initializes certain settings based on the operating system language. This method is intended
        /// to be called before accessing settings that may require migration or initialization.</remarks>
        private static void LoadSettingsIfNeeded()
        {
            if (!Settings1.Default.HasSettingsUpgraded)
            {
                bool formerState = Settings1.Default.HasSettingsUpgraded;
                Settings1.Default.Upgrade();
                Settings1.Default.Save();
                bool currentState = Settings1.Default.HasSettingsUpgraded;
                if (formerState == currentState) // This means no saved settings is found from previous versions
                {
                    isFirstRun = true; // Mark as first run if no saved settings is found
                    SetLanguageBasedOnOSLanguage(); // Set language based on OS language if no saved settings is found
                }
                Settings1.Default.HasSettingsUpgraded = true;
                Settings1.Default.Save();
            }
        }
    }
}