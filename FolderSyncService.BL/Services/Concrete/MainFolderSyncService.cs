using FolderSyncService.BL.Services.Abstract;
using System;
using System.Timers;

namespace FolderSyncService.BL.Services.Concrete
{
    public class MainFolderSyncService : IService
    {
        private const int SYNC_PERIOD_MS = 2_000;
        private readonly Timer _timer;

        public MainFolderSyncService()
        {
            _timer = new Timer(SYNC_PERIOD_MS)
            {
                AutoReset = true
            };
            _timer.Elapsed += TimerElapsedHandler;
        }

        private void TimerElapsedHandler(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Hi!");
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
