using FolderSyncService.BL.Services.Concrete;
using FolderSyncService.Properties;
using System;
using Topshelf;

namespace FolderSyncService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var exitCodeTopshelf = HostFactory.Run(x =>
            {
                x.Service<MainFolderSyncService>(s =>
                {
                    s.ConstructUsing(sync => new MainFolderSyncService(Settings.Default.SyncPeriodMs, Settings.Default.SourceFolderPath, Settings.Default.DestinationFolderPath));
                    s.WhenStarted(sync => sync.Start());
                    s.WhenStopped(sync => sync.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName(Settings.Default.ServiceName);
                x.SetDisplayName(Settings.Default.DisplayName);
                x.SetDescription(Settings.Default.Description);
            });

            var exitCodeValue = (int)Convert.ChangeType(exitCodeTopshelf, exitCodeTopshelf.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
