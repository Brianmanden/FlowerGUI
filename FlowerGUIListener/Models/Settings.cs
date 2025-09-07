using System;
using System.IO;
using System.Text.Json;

namespace FlowerGUIListener.Models
{
    public class Settings
    {
        public string HotkeyCombination { get; set; } = "Ctrl+RightClick";
        public bool StartWithWindows { get; set; } = false;
        public bool ShowNotifications { get; set; } = true;
        public string NotesDirectory { get; set; } = "";
        public string ScreenshotsDirectory { get; set; } = "";

        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FlowerGUI",
            "settings.json");

        public Settings()
        {
            // Set default directories
            NotesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ScreenshotsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static Settings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
            }

            return new Settings();
        }

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
    }
}
