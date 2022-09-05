using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.Threads;

namespace BlazorFFMPEG.Backend.Modules.ServerLoad
{
    public class ServerLoadAnalyzer
    {
        public struct ServerLoadTimepoint
        {
            public DateTime lastLoadTime { get; init; }
            public long currentLoad { get; init; }
        }
        
        #region Singleton
        private static ServerLoadAnalyzer instance;
        
        public static ServerLoadAnalyzer getInstance()
        {
            return instance ??= new ServerLoadAnalyzer();
        }

        private ServerLoadAnalyzer()
        {
            SetIntervalUtil.SetInterval(() =>
            {
                double averageLoad = calculateAverageLoad(new TimeSpan(0, 1, 0));
                Logger.w($"Average load: {averageLoad}%");
            }, new TimeSpan(0,1,0));
        }
        
        #endregion

        public List<ServerLoadTimepoint> serverLoadHistory = new List<ServerLoadTimepoint>();
        
        public void handleLoad(long duration)
        {
            ServerLoadTimepoint loadTimepoint = new ServerLoadTimepoint() {lastLoadTime = DateTime.Now, currentLoad = duration};
            serverLoadHistory.Add(loadTimepoint);
        }

        public double calculateAverageLoad(TimeSpan duration)
        {
            List<ServerLoadTimepoint> serverLoadTimepoints = serverLoadHistory.FindAll(s => s.lastLoadTime + duration > DateTime.Now);

            long totalLoad = 0;
            foreach (ServerLoadTimepoint serverLoadTimepoint in serverLoadTimepoints)
            {
                totalLoad += serverLoadTimepoint.currentLoad;
            }

            return (totalLoad*100) / (duration.TotalSeconds * 1000);
        }

        public void reset()
        {
            this.serverLoadHistory = new List<ServerLoadTimepoint>();
        }
    }
}