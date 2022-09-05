using System.Collections;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Events;
using EinfachAlex.Utils.Threads;
using Microsoft.EntityFrameworkCore;

namespace BlazorFFMPEG.Backend.Modules.Jobs;

public class QueueScannerJob
{
    private static QueueScannerJob instance;
    private List<EncodeJob> encodeJobs;

    public static QueueScannerJob getInstance()
    {
        return instance ??= new QueueScannerJob();
    }

    //Raised for every encode job that is found in the queue
    public delegate void SampleEventHandler(object sender, QueueScanItemFoundEventArgs e);
    public event SampleEventHandler encodeJobFoundInQueue;
    
    private bool scanningEnabled = false;
    
    public void startScanning()
    {
        scanningEnabled = true;

        SetIntervalUtil.SetInterval(() =>
            {
                this.scan();
            },
            TimeSpan.FromMinutes(1));
    }
    
    private async Task scan()
    {
        List<EncodeJob> waitingJobs = await getWaitingJobs();

        raiseEvents(waitingJobs);
    }
    
    public async Task<List<EncodeJob>> getWaitingJobs()
    {
        using (databaseContext databaseContext = new databaseContext())
        {
            encodeJobs = databaseContext.EncodeJobs.Where(e => e.Status == (int)EEncodingStatus.NEW).ToList();
        }

        return encodeJobs;
    }
    
    private void raiseEvents(List<EncodeJob> waitingJobs)
    {
        foreach (EncodeJob encodeJob in waitingJobs)
        {
            onEncodeJobFoundInQueue(new QueueScanItemFoundEventArgs(encodeJob));
        }
    }

    private void onEncodeJobFoundInQueue(QueueScanItemFoundEventArgs e)
    {
        SampleEventHandler handler = encodeJobFoundInQueue;
        handler?.Invoke(this, e);
    }
    
    /*public async Task<EncodeJob> getNextWaitingJob()
    {
        //Todo logic which job to get first

        EncodeJob encodeJob;
        using (databaseContext databaseContext = new databaseContext())
        {
            encodeJob = await databaseContext.EncodeJobs.FirstAsync(e => e.Status == (int)EEncodingStatus.NEW);
        }

        return encodeJob;
    }

    public async Task startJob(EncodeJob encodeJob)
    {
        await new FFMPEG.FFMPEG().startEncode(encodeJob.Path, encodeJob.Path);
    }

    public async Task startNextWaitingJob()
    {
        EncodeJob nextWaitingJob = await getNextWaitingJob();

        await startJob(nextWaitingJob);
    }*/
}