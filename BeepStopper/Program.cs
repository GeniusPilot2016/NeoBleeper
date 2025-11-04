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
using Windows.ApplicationModel.Activation;

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
            Debug.WriteLine("Beep stopper is starting...");
            checkAndPlaceInpOutX64(); // Ensure InpOutx64.dll is present
            loadSettings();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string query = "SELECT * FROM Win32_PNPEntity Where DeviceID like '%PNP0800%'";
            Debug.WriteLine("Checking for system speaker presence...");
            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(query);
            ManagementObjectCollection number_of_system_speaker_devices = searcher1.Get();
            bool is_system_speaker_present = number_of_system_speaker_devices.Count >= 1;
            if (is_system_speaker_present)
            {
                Debug.WriteLine("System speaker is present. Starting the beep stopper application.");
                Application.Run(new main_window());
            }
            else
            {
                Debug.WriteLine("System speaker output is not present or non-standard system speaker output is present. Beep stopper may cause instability or undesirable behaviors.");
                DialogResult dialogResult = MessageBox.Show(Resources.SystemSpeakerNotPresentMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        Debug.WriteLine("User chose to continue despite the warning. Starting the beep stopper application.");
                        Application.Run(new main_window());
                        break;
                    case DialogResult.No:
                        Debug.WriteLine("User chose not to continue. Exiting the application.");
                        break;
                    default:
                        Debug.WriteLine("Unexpected dialog result. Exiting the application.");
                        break;
                }
            }
        }
        private static void loadSettings()
        {
            var synchronizedSettings = SynchronizedSettings.Load(false);
            UIHelper.setLanguageByName(synchronizedSettings.Language);
            Debug.WriteLine($"Beep stopper is starting with language: {synchronizedSettings.Language}");
        }
        private static void checkAndPlaceInpOutX64()
        {
            var inpOutX64File = NeoBleeper.Properties.Resources.inpoutx64; // InpOutx64.dll binary resource
            var inpOutX64Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InpOutx64.dll");
            if (!isInpOutX64PresentAndValid()) // Check if InpOutx64.dll is present and valid
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
        private static bool isInpOutX64PresentAndValid() // Check if InpOutx64.dll is present and valid by comparing SHA256 hash
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