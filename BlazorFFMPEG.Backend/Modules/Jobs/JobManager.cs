using BlazorFFMPEG.Backend.Database;
using Microsoft.EntityFrameworkCore;

namespace BlazorFFMPEG.Backend.Modules.Jobs;

public class JobManager
{
    private readonly ILogger _logger;
    private readonly QueueScannerJob _queueScannerJob;

    public JobManager(ILogger<JobManager> logger, QueueScannerJob queueScannerJob)
    {
        _logger = logger;
        _queueScannerJob = queueScannerJob;
    }

    private bool jobThreadsStarted = false;
    private string? connectionString;

    public void startJobThreads(databaseContext databaseContext)
    {
        connectionString = databaseContext.Database.GetConnectionString();
        
        if (jobThreadsStarted) return;
        
        startQueueScanner(connectionString);
        jobThreadsStarted = true;
    }

    private void startQueueScanner(string connectionString)
    {
        Thread thread = new Thread(() => _queueScannerJob.startScanning(connectionString));
        thread.Start();
    }
}