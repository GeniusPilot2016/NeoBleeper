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

using System.Diagnostics;

namespace NeoBleeper
{
    public static class check_system_theme
    {
        public static bool IsDarkTheme()
        {
            try
            {
                Process process = new();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments =
                    @"/C reg query HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize\";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string[] keys = output.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < keys.Length; i++)
                {
                    if (keys[i].Contains("AppsUseLightTheme"))
                    {
                        return keys[i].EndsWith("0");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Error checking system theme: {ex.Message}", Logger.LogTypes.Error);
            }

            return false;
        }
    }
}
