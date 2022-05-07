using FolderSyncService.BL.Services.Abstract;
using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace FolderSyncService.BL.Services.Concrete
{
    public class MainFolderSyncService : IService
    {
        private readonly Timer _timer;
        private readonly double _syncPeriodMs;
        private readonly string _srcFolder;
        private readonly string _dstFolder;

        public MainFolderSyncService(double syncPeriodMs, string srcFolder, string dstFolder)
        {
            _syncPeriodMs = syncPeriodMs;
            _srcFolder = srcFolder;
            _dstFolder = dstFolder;
            if (!Directory.Exists(_srcFolder))
                throw new DirectoryNotFoundException($"Don't exist source folder {_srcFolder}");
            _timer = new Timer(syncPeriodMs)
            {
                AutoReset = true
            };
            _timer.Elapsed += TimerElapsedHandler;
        }
        
        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void TimerElapsedHandler(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Start sync...");

            try
            {
                var syncCount = Sync();
                Console.WriteLine($"Synced {syncCount} files");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Finish sync...");
        }

        private int Sync()
        {
            var syncCount = 0;
            var infos = Directory.GetFiles(_srcFolder).Select(i => new FileInfo(i));
            foreach (var info in infos)
            {
                if (!IsActualDateSync(info))
                    continue;
                var dstFilePath = GetUnicueFullPath(Path.Combine(_dstFolder, info.Name));
                File.Copy(info.FullName, dstFilePath, false);
                syncCount++;
            }
            return syncCount;
        }

        private bool IsActualDateSync(FileInfo fileInfo)
        {
            var lastDate = GetLastFileDate(fileInfo);
            return DateTime.Now.AddMilliseconds(-_syncPeriodMs) <= lastDate;
        }

        private DateTime GetLastFileDate(FileInfo fileInfo)
        {
            var lastDate = fileInfo.CreationTime > fileInfo.LastWriteTime ? fileInfo.CreationTime : fileInfo.LastWriteTime;
            //if (fileInfo.LastAccessTime > lastDate)
            //    return fileInfo.LastAccessTime;
            return lastDate;
        }

        private string GetUnicueFullPath(string fullPath)
        {
            var count = 1;
            var fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            var extension = Path.GetExtension(fullPath);
            var path = Path.GetDirectoryName(fullPath);
            var newFullPath = fullPath;
            while (File.Exists(newFullPath))
            {
                string tempFileName = $"{fileNameOnly} ({count++})";
                newFullPath = Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }
    }
}
