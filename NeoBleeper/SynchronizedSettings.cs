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
using System.Reflection;
using System.Text.Json;

namespace NeoBleeper
{
    public class SynchronizedSettings
    {
        private static readonly string SettingsFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SharedSettings.json");

        public string Language { get; set; } = "English";
        public int Theme { get; set; } = 0;

        /// <summary>
        /// Loads the synchronized settings from the settings file, or creates default settings if the file does not
        /// exist.
        /// </summary>
        /// <remarks>If the settings file is missing or cannot be read, this method creates and returns a
        /// new SynchronizedSettings instance with default values. Status messages about the operation are either logged
        /// to the application log file or written to the debug console, depending on the value of logToFile.</remarks>
        /// <param name="logToFile">true to log status messages to the application log file; false to output status messages to the debug
        /// console instead.</param>
        /// <returns>A SynchronizedSettings instance loaded from the settings file, or a new instance with default values if the
        /// file does not exist or cannot be read.</returns>
        public static SynchronizedSettings Load(bool logToFile = true)
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                {
                    // Create default settings if the file doesn't exist
                    var defaultSettings = new SynchronizedSettings();
                    defaultSettings.Save(logToFile);
                    if (logToFile)
                        Logger.Log("Settings file not found. Created default settings.", Logger.LogTypes.Info);
                    else
                        Debug.WriteLine("Settings file not found. Created default settings.");
                    return defaultSettings;
                }

                var json = File.ReadAllText(SettingsFilePath, System.Text.Encoding.UTF8); // Ensure UTF-8 encoding
                if (logToFile)
                    Logger.Log("Synchronized settings loaded successfully.", Logger.LogTypes.Info);
                else
                    Debug.WriteLine("Synchronized settings loaded successfully.");
                return JsonSerializer.Deserialize<SynchronizedSettings>(json, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Allow direct UTF-8 characters
                }) ?? new SynchronizedSettings();
            }
            catch (Exception ex)
            {
                if (logToFile)
                    Logger.Log($"Failed to load settings: {ex.Message}", Logger.LogTypes.Error);
                else
                    Debug.WriteLine($"Failed to load settings: {ex.Message}");
                return new SynchronizedSettings();
            }
        }

        /// <summary>
        /// Saves the current settings to persistent storage as a JSON file.
        /// </summary>
        /// <remarks>The settings are serialized in indented JSON format using UTF-8 encoding. If an error
        /// occurs during saving, the error is logged either to the main logger or to the debug output, depending on the
        /// value of saveToFile.</remarks>
        /// <param name="saveToFile">true to write the settings to the settings file; false to perform the operation without writing to disk. If
        /// false, errors are logged to the debug output instead of the main logger.</param>
        public void Save(bool saveToFile = true)
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
                if (saveToFile)
                    Logger.Log($"Failed to save settings: {ex.Message}", Logger.LogTypes.Error);
                else
                    Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
    }
}