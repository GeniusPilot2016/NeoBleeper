using System.IO;
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