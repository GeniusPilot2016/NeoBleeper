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

using System.Reflection;
using System.Text.Json;

namespace NeoBleeper
{
    public class SynchronizedSettings
    {
        private static readonly string SettingsFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SharedSettings.json");

        public string Language { get; set; } = "English";
        public int Theme { get; set; } = 0;

        public static SynchronizedSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    // Create default settings if the file doesn't exist
                    var defaultSettings = new SynchronizedSettings();
                    defaultSettings.Save();
                    Logger.Log("Settings file not found. Created default settings.", Logger.LogTypes.Info);
                    return defaultSettings;
                }

                var json = File.ReadAllText(SettingsFilePath, System.Text.Encoding.UTF8); // Ensure UTF-8 encoding
                Logger.Log("Synchronized settings loaded successfully.", Logger.LogTypes.Info);
                return JsonSerializer.Deserialize<SynchronizedSettings>(json, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Allow direct UTF-8 characters
                }) ?? new SynchronizedSettings();
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to load settings: {ex.Message}", Logger.LogTypes.Error);
                return new SynchronizedSettings();
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Allow direct UTF-8 characters
                });
                File.WriteAllText(SettingsFilePath, json, System.Text.Encoding.UTF8); // Ensure UTF-8 encoding
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to save settings: {ex.Message}", Logger.LogTypes.Error);
            }
        }
    }
}