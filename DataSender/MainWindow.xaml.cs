using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using DataSender.Models;
using DataSender.Services;
using DataSender.Views;

namespace DataSender
{
    public partial class MainWindow : Window
    {
        private FileSystemWatcher? _watcher;
        private AppSettings _settings;
        private bool _isMonitoring = false;
        public MainWindow()
        {
            InitializeComponent();

            _settings = AppSettings.Load();
            Log("Settings loaded.");
            SetStatus("Idle", Colors.Gray);
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (_isMonitoring)
            {
                Log("Already monitoring.");
                return;
            }

            if (!Directory.Exists(_settings.WatchDirectory))
            {
                MessageBox.Show("Watch directory does not exist.");
                return;
            }

            _watcher = new FileSystemWatcher(_settings.WatchDirectory)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite,
                //Filter = $"*{_settings.FileSuffix}"
            };

            _watcher.Created += OnFileCreated;
            _isMonitoring = true;
            Log($"Started monitoring: {_settings.WatchDirectory}");
            SetStatus("Monitoring", Colors.Green);
        }

        private void StopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
                _isMonitoring = false;
                Log("Stopped monitoring.");
                SetStatus("Stopped", Colors.Red);
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                string originalName = Path.GetFileNameWithoutExtension(e.Name);
                string extension = Path.GetExtension(e.Name);

                var renameDialog = new RenameDialog(originalName)
                {
                    Owner = this
                };

                if (renameDialog.ShowDialog() != true)
                {
                    Log($"Skipped file: {e.Name} (user canceled rename)");
                    return;
                }

                string renamedFile = $"{renameDialog.EnteredName}-{_settings.FileSuffix}{extension}";
                string destPath = Path.Combine(_settings.DestinationDirectory, renamedFile);
                string backupPath = Path.Combine(_settings.BackupDirectory, DateTime.Now.ToString("yyyy-MM-dd"), renamedFile);

                Task.Run(() =>
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(backupPath)!);
                        File.Copy(e.FullPath, backupPath, overwrite: true);

                        Directory.CreateDirectory(_settings.DestinationDirectory);
                        File.Move(e.FullPath, destPath, overwrite: true);

                        Dispatcher.Invoke(() =>
                        {
                            Log($"Transfer complete: {renamedFile}");
                            ToastService.ShowToast("Transfer Complete", renamedFile);
                        });
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Log($"Transfer failed: {renamedFile} — {ex.Message}");
                            ToastService.ShowToast("Transfer Failed", renamedFile);
                        });
                    }
                });
            });
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings.Clone())
            {
                Owner = this
            };

            if (settingsWindow.ShowDialog() == true)
            {
                _settings = settingsWindow.Settings;
                _settings.Save();
                Log("Settings saved.");
            }
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogList.Items.Add($"[{timestamp}] {message}");

            // Keep the most recent log visible
            LogList.ScrollIntoView(LogList.Items[LogList.Items.Count - 1]);
        }

        private void SetStatus(string text, System.Windows.Media.Color color)
        {
            StatusText.Text = $"Status: {text}";
            StatusText.Foreground = new System.Windows.Media.SolidColorBrush(color);
        }
    }
}
