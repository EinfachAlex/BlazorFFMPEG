namespace BlazorFFMPEG.Backend.Modules.Jobs;

public class JobManager
{
    private static JobManager instance;

    public static JobManager getInstance()
    {
        return instance ??= new JobManager();
    }

    public void startJobThreads()
    {
        startQueueScanner();
    }

    private void startQueueScanner()
    {
        Thread thread = new Thread(QueueScannerJob.getInstance().startScanning);
        thread.Start();
    }
}