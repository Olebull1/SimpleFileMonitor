using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DataSender.Services
{
    public class RemovableMonitorService
    {
        private readonly string _path;
        private readonly HashSet<string> _seen = [];
        private readonly System.Timers.Timer _pollTimer;

        public event Action<string>? FileDetected;

        private bool _firstRun = true;
        public RemovableMonitorService(string path, double intervalSeconds = 5)
        {
            _path = path;
            _pollTimer = new System.Timers.Timer(intervalSeconds * 1000);
            _pollTimer.Elapsed += (s, e) => Poll();
            _pollTimer.AutoReset = true;
        }

        public void Start() => _pollTimer.Start();
        public void Stop() => _pollTimer.Stop();

        private void Poll()
        {
            if (!Directory.Exists(_path)) return;

            string[] files;
            try
            {
                files = Directory.GetFiles(_path);
            }
            catch
            {
                return; // If access fails mid-poll (e.g. drive removed), just skip
            }

            foreach (var file in files)
            {
                if (_seen.Add(file))
                {
                    if(_firstRun == false)
                        FileDetected?.Invoke(file);
                }
            }
            _firstRun = false;
        }
    }
}
