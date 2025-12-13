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

using BeepStopper.Properties;
using NeoBleeper;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using static UIHelper;

namespace BeepStopper
{
    internal static class Program
    {
        public static int themeIndex = 0;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Debug.WriteLine("Beep stopper is starting...");

            CheckAndPlaceInpOutX64(); // Ensure InpOutx64.dll is present
            LoadSettings();
            ThemeManager.Initialize();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string query = "SELECT * FROM Win32_PNPEntity Where DeviceID like '%PNP0800%'";
            Debug.WriteLine("Checking for system speaker presence...");
            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(query);
            ManagementObjectCollection numberOfSystemSpeakerDevices = searcher1.Get();
            bool isSystemSpeakerPresent = numberOfSystemSpeakerDevices.Count >= 1;
            if (isSystemSpeakerPresent)
            {
                Debug.WriteLine("System speaker is present. Starting the beep stopper application.");
                Application.Run(new MainWindow());
            }
            else
            {
                Debug.WriteLine("System speaker output is not present or non-standard system speaker output is present. Beep stopper may cause instability or undesirable behaviors.");
                DialogResult dialogResult = MessageForm.Show(Program.themeIndex, Resources.SystemSpeakerNotPresentMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        Debug.WriteLine("User chose to continue despite the warning. Starting the beep stopper application.");
                        Application.Run(new MainWindow());
                        break;
                    case DialogResult.No:
                        Debug.WriteLine("User chose not to continue. Exiting the application.");
                        break;
                    default:
                        Debug.WriteLine("Unexpected dialog result. Exiting the application.");
                        break;
                }
            }
            ThemeManager.Cleanup();
            Debug.WriteLine("Beep stopper application has exited.");
        }

        /// <summary>
        /// Loads application settings from the synchronized settings source and applies the theme and language
        /// preferences.
        /// </summary>
        /// <remarks>This method updates the application's theme and language based on the current
        /// synchronized settings. It should be called during application startup or when settings need to be
        /// refreshed.</remarks>
        private static void LoadSettings()
        {
            var SynchronizedSettings = NeoBleeper.SynchronizedSettings.Load(false);
            themeIndex = SynchronizedSettings.Theme;
            UIHelper.SetLanguageByName(SynchronizedSettings.Language);
            Debug.WriteLine($"Beep stopper is starting with language: {SynchronizedSettings.Language}");
        }

        /// <summary>
        /// Checks if InpOutx64.dll is present and valid; if not, places the DLL file from embedded resources.
        /// </summary>
        /// <remarks> This method ensures that the InpOutx64.dll file is available in the application's base directory.
        /// If the file is missing or corrupted, it writes the DLL from the embedded resources to the file system.
        /// </remarks>
        private static void CheckAndPlaceInpOutX64()
        {
            var inpOutX64File = NeoBleeper.Properties.Resources.inpoutx64; // InpOutx64.dll binary resource
            var inpOutX64Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InpOutx64.dll");
            if (!IsInpOutX64PresentAndValid()) // Check if InpOutx64.dll is present and valid
                                               // If not present or broken, place the DLL file
            {
                try
                {
                    Debug.WriteLine("InpOutx64.dll not found or is corrupted. Placing the DLL file...");
                    File.WriteAllBytes(inpOutX64Path, inpOutX64File);
                    Debug.WriteLine("InpOutx64.dll placed successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error placing InpOutx64.dll: {ex.Message}");
                }
            }
            else
            {
                Debug.WriteLine("InpOutx64.dll already exists and valid. No action needed.");
            }
        }

        /// <summary>
        /// Determines whether the InpOutx64.dll file is present in the application's base directory and matches the
        /// expected SHA256 hash.
        /// </summary>
        /// <remarks>This method verifies both the presence and integrity of InpOutx64.dll by comparing
        /// its SHA256 hash to a known good value. Use this check before attempting to interact with the DLL to ensure
        /// compatibility and prevent potential errors due to tampering or corruption.</remarks>
        /// <returns>true if InpOutx64.dll exists and its SHA256 hash matches the embedded reference; otherwise, false.</returns>
        private static bool IsInpOutX64PresentAndValid() // Check if InpOutx64.dll is present and valid by comparing SHA256 hash
        {
            var inpOutX64Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InpOutx64.dll");
            if (File.Exists(inpOutX64Path)) // Check if file exists
            {
                string expectedSHA256Hash = string.Empty;
                string actualSHA256Hash = string.Empty;
                using (SHA256 sha256 = SHA256.Create()) // Create SHA256 instance to compute hash
                {
                    byte[] hashBytes = sha256.ComputeHash(NeoBleeper.Properties.Resources.inpoutx64);
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
    }
}