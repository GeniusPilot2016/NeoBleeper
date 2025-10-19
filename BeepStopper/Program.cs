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
using System.Management;
using static NeoBleeper.Logger;

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
            Logger.Log("Beep stopper is starting...", LogTypes.Info);
            loadSettings();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            string query = "SELECT * FROM Win32_PNPEntity Where DeviceID like '%PNP0800%'";
            Logger.Log("Checking for system speaker presence...", LogTypes.Info);
            ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(query);
            ManagementObjectCollection number_of_system_speaker_devices = searcher1.Get();
            bool is_system_speaker_present = number_of_system_speaker_devices.Count >= 1;
            ApplicationConfiguration.Initialize();
            if (is_system_speaker_present)
            {
                Logger.Log("System speaker is present. Starting the beep stopper application.", LogTypes.Info);
                Application.Run(new main_window());
            }
            else
            {
                Logger.Log("System speaker output is not present or non-standard system speaker output is present. Beep stopper may cause instability or undesirable behaviors.", LogTypes.Warning);
                DialogResult dialogResult = MessageBox.Show(Resources.SystemSpeakerNotPresentMessage, string.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        Logger.Log("User chose to continue despite the warning. Starting the beep stopper application.", LogTypes.Info);
                        Application.Run(new main_window());
                        break;
                    case DialogResult.No:
                        Logger.Log("User chose not to continue. Exiting the application.", LogTypes.Info);
                        break;
                    default:
                        Logger.Log("Unexpected dialog result. Exiting the application.", LogTypes.Warning);
                        break;
                }
            }
        }
        private static void loadSettings()
        {
            var synchronizedSettings = SynchronizedSettings.Load();
            UIHelper.setLanguageByName(synchronizedSettings.Language);
            Logger.Log($"Beep stopper is starting with language: {synchronizedSettings.Language}", LogTypes.Info);
        }
    }
}