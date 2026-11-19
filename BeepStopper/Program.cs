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

            LoadSettings();
            ThemeManager.Initialize();
            if (!ProgramPrecheck.IsRanAsAdmin())
            {
                MessageForm.Show(Program.themeIndex, Resources.AdminPrivilegesRequiredMessage, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
            if (!ProgramPrecheck.IsPawnIOInstalled())
            {
                PawnIOWrapper.InstallPawnIO();
            }
            if (!ProgramPrecheck.IsPawnIOInstalled())
            {
                MessageForm.Show(Program.themeIndex, Resources.PawnIOIsNotInstalled, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
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

    }
}