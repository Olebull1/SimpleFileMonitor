using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.IO;

namespace DataSender.Services
{
    public class FileMonitorService
    {
        private FileSystemWatcher? _watcher;
        public bool IsRunning => _watcher != null;

        public event Action<string>? FileDetected;

        public void Start(string watchDirectory)
        {
            if (_watcher != null)
                return;

            _watcher = new FileSystemWatcher(watchDirectory)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite,
                Filter = "*.*" // Catch everything
            };

            _watcher.Created += OnFileCreated;
        }

        public void Stop()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            FileDetected?.Invoke(e.FullPath);
        }
    }
}