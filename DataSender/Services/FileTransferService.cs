using System;
using System.IO;
using System.Threading.Tasks;
using DataSender.Models;

namespace DataSender.Services
{
    public class FileTransferService
    {
        private readonly AppSettings _settings;

        public FileTransferService(AppSettings settings) => _settings = settings;

        public void BackupAndCopy(
            string sourcePath,
            string renamedFileName,
            Action<string>? log = null,
            Action<string, bool>? notify = null)
        {
            Task.Run(() =>
            {
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                string backupDir = Path.Combine(_settings.BackupDirectory, today);
                string destDir = _settings.DestinationDirectory;

                string backupPath = Path.Combine(backupDir, renamedFileName);
                string destPath = Path.Combine(destDir, renamedFileName);

                try
                {
                    Directory.CreateDirectory(backupDir);
                    Directory.CreateDirectory(destDir);

                    File.Copy(sourcePath, backupPath, overwrite: true);
                    if (_settings.IsRemovable)
                    {
                        File.Copy(sourcePath, destPath, overwrite: true);
                    }
                    else
                    {
                        File.Move(sourcePath, destPath, overwrite: true);
                    }

                        log?.Invoke($"Transfer complete: {renamedFileName}");
                    notify?.Invoke(renamedFileName, true);
                }
                catch (Exception ex)
                {
                    log?.Invoke($"Transfer failed: {renamedFileName} — {ex.Message}");
                    notify?.Invoke(renamedFileName, false);
                }
            });
        }
    }
}
