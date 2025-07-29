using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataSender.Models
{
    public class AppSettings
    {
        public string WatchDirectory { get; set; } = @"C:\Watched";
        public string FileSuffix { get; set; } = "VF";
        public string DestinationDirectory { get; set; } = @"C:\Destination";
        public string BackupDirectory { get; set; } = @"C:\Backups";
        public bool IsRemovable { get; set; } = false;

        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FileMonitorApp", "settings.json");

        public AppSettings Clone()
        {
            return new AppSettings
            {
                WatchDirectory = this.WatchDirectory,
                FileSuffix = this.FileSuffix,
                DestinationDirectory = this.DestinationDirectory,
                BackupDirectory = this.BackupDirectory,
                IsRemovable = this.IsRemovable
            };
        }

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }

            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch { }
        }
    }
}
