using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Events;
using EinfachAlex.Utils.Logging;

namespace BlazorFFMPEG.Backend.Modules.Jobs;

public class EncodeJobManager
{
    private readonly FFMPEG.FFMPEG _ffmpeg;
    private readonly QueueScannerJob _queueScannerJob;

    public EncodeJobManager(FFMPEG.FFMPEG ffmpeg, ILogger<EncodeJobManager> logger, QueueScannerJob queueScannerJob)
    {
        _ffmpeg = ffmpeg;
        _queueScannerJob = queueScannerJob;
        
        _queueScannerJob.encodeJobFoundInQueue += encodeJobFoundInQueue;
    }

    private List<Thread> activeThreads = new List<Thread>();

    private void encodeJobFoundInQueue(object? sender, QueueScanItemFoundEventArgs e)
    {
        using (databaseContext databaseContext = new databaseContext())
        {
            databaseContext.Attach(e.job);
            
            e.job.setStatus(databaseContext, EEncodingStatus.WORKING);

            Thread thread = new Thread(() => startEncodeThread(e));
            thread.Start();
        
            this.activeThreads.Add(thread);

            databaseContext.Update(e.job);
            
            databaseContext.SaveChanges();
        }
    }

    async Task startEncodeThread(QueueScanItemFoundEventArgs e)
    {
        using (databaseContext databaseContext = new databaseContext())
        {
            databaseContext.Attach(e.job);
            
            await _ffmpeg.startEncode(e.job.Path, e.job.Codec, e.job.Qualitymethod, e.job.Jobid);
            
            e.job.setStatus(databaseContext, EEncodingStatus.FINISHED);
            databaseContext.Update(e.job);

            databaseContext.SaveChanges();
            await _queueScannerJob.forceScan(databaseContext);
        }
    }
    
    public int getNumberActiveThreads()
    {
        List<Thread> threadsToRemove = new List<Thread>();
        foreach (Thread activeThread in this.activeThreads)
        {
            if (!activeThread.IsAlive)
            {
                Logger.v($"Removed thread {activeThread.ManagedThreadId} from active thread queue because it is finished");
                
                threadsToRemove.Add(activeThread);
            }
        }
        
        foreach (Thread thread in threadsToRemove)
        {
            this.activeThreads.Remove(thread);
        }
        
        int numberActiveThreads = this.activeThreads.Count;
        Logger.i($"There are {numberActiveThreads} threads active");

        return numberActiveThreads;
    }
}