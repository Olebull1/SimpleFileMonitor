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
        private AppSettings _settings;
        private FileMonitorService? _fileMonitor;
        private RemovableMonitorService? _removableMonitor;
        private FileTransferService _transferService;

        public MainWindow()
        {
            InitializeComponent();

            _settings = AppSettings.Load();
            _transferService = new FileTransferService(_settings);

            Log("Settings loaded.");
            SetStatus("Idle", Colors.Gray);
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (_settings.IsRemovable)
            {
                if (_removableMonitor != null)
                {
                    Log("Removable monitor is already running.");
                    return;
                }

                _removableMonitor = new RemovableMonitorService(_settings.WatchDirectory, 1);
                _removableMonitor.FileDetected += OnFileDetected;
                _removableMonitor.Start();

                Log($"Started removable polling on {_settings.WatchDirectory}");
                SetStatus("Polling Removable Directory", Colors.Green);
            }
            else
            {
                if (_fileMonitor?.IsRunning == true)
                {
                    Log("File monitor is already running.");
                    return;
                }

                _fileMonitor = new FileMonitorService();
                _fileMonitor.FileDetected += OnFileDetected;
                _fileMonitor.Start(_settings.WatchDirectory);

                Log($"Started file monitor on {_settings.WatchDirectory}");
                SetStatus("Monitoring", Colors.Green);
            }
        }

        private void StopMonitoring_Click(object sender, RoutedEventArgs e)
        {
            _fileMonitor?.Stop();
            _removableMonitor?.Stop();

            _fileMonitor = null;
            _removableMonitor = null;

            Log("Monitoring stopped.");
            SetStatus("Stopped", Colors.Red);
        }

        private void OnFileDetected(string fullPath)
        {
            Dispatcher.Invoke(() =>
            {
                string fileName = Path.GetFileName(fullPath);
                string baseName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);

                var dialog = new RenameDialog(baseName) { Owner = this };
                if (dialog.ShowDialog() != true)
                {
                    Log($"Skipped file: {fileName} (rename canceled)");
                    return;
                }

                string renamedFile = $"{dialog.EnteredName}-{_settings.FileSuffix}{extension}";

                _transferService.BackupAndCopy(
                    sourcePath: fullPath,
                    renamedFileName: renamedFile,
                    log: Log,
                    notify: (file, success) =>
                    {
                        string title = success ? "Transfer Complete" : "Transfer Failed";
                        ToastService.ShowToast(title, file);
                    });
            });
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(_settings.Clone()) { Owner = this };
            if (settingsWindow.ShowDialog() == true)
            {
                _settings = settingsWindow.Settings;
                _settings.Save();
                _transferService = new FileTransferService(_settings);
                Log("Settings updated.");
            }
        }

        private void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                LogList.Items.Add($"[{timestamp}] {message}");
                LogList.ScrollIntoView(LogList.Items[^1]);
            });
        }

        private void SetStatus(string text, Color color)
        {
            StatusText.Text = $"Status: {text}";
            StatusText.Foreground = new SolidColorBrush(color);
        }
    }
}