using System.Net.WebSockets;
using System.Text;
using BlazorFFMPEG.Backend.Controllers.Get;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Events;
using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.Threads;

namespace BlazorFFMPEG.Backend.Modules.Jobs;

public class QueueScannerJob
{
    private readonly databaseContext _context;
    private readonly ILogger _logger;
    private readonly FFMPEG.FFMPEG _ffmpeg;
    private readonly IServiceProvider _serviceProvider;

    public QueueScannerJob(ILogger<QueueScannerJob> logger, FFMPEG.FFMPEG ffmpeg, IServiceProvider services)
    {
        //_context = context;
        _logger = logger;
        _ffmpeg = ffmpeg;
        _serviceProvider = services;
    }

    private List<EncodeJob> encodeJobs;
    private const int MAX_THREADS = 2;

    //Raised for every encode job that is found in the queue
    public delegate void SampleEventHandler(object sender, QueueScanItemFoundEventArgs e);
    public event SampleEventHandler encodeJobFoundInQueue;

    private bool scanningEnabled = false;

    public void startScanning(string connectionString)
    {
        using (databaseContext databaseContext = new databaseContext(connectionString))
        {
            scanningEnabled = true;

            checkUnfinishedJobs(databaseContext);

            SetIntervalUtil.SetInterval(async () =>
                {
                    //Needed because variable in outer scope will get disposed
                    using (databaseContext databaseContext = new databaseContext())
                    {
                        int numberActiveThreads = _serviceProvider.GetRequiredService<EncodeJobManager>().getNumberActiveThreads();

                        if (numberActiveThreads <= MAX_THREADS)
                        {
                            await this.scan(databaseContext);
                        }
                    }
                },
                TimeSpan.FromMinutes(1));

            this.forceScan(databaseContext); //Scan instantly at program start
        }
    }

    private void checkUnfinishedJobs(databaseContext databaseContext)
    {
        {
            //Jobs with status WORKING are unfinished (this method is called directly after the program starts)
            List<EncodeJob> unfinishedEncodeJobs = databaseContext.EncodeJobs.Where(j => j.Status == Convert.ToInt32(EEncodingStatus.WORKING)).ToList();

            foreach (EncodeJob unfinishedEncodeJob in unfinishedEncodeJobs)
            {
                unfinishedEncodeJob.resetStatus(databaseContext);
            }

            databaseContext.SaveChanges();
        }
    }

    private async Task scan(databaseContext databaseContext)
    {
        List<EncodeJob> waitingJobs = await getWaitingJobs(databaseContext);

        byte[] websocketMessage = Encoding.ASCII.GetBytes($"Scanned the queue, found {waitingJobs.Count} jobs");
        WebSocketController.websocketServer?.SendAsync(new ArraySegment<byte>(websocketMessage, 0, websocketMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);

        raiseEvents(databaseContext, waitingJobs);
    }

    private bool maxThreadsReached()
    {
        bool maxThreadsReached = _serviceProvider.GetRequiredService<EncodeJobManager>().getNumberActiveThreads() >= MAX_THREADS;

        return maxThreadsReached;
    }

    public async Task<List<EncodeJob>> getWaitingJobs(databaseContext databaseContext)
    {
        encodeJobs = databaseContext.EncodeJobs.Where(e => e.Status == (int)EEncodingStatus.NEW).ToList();

        return encodeJobs;
    }

    private void raiseEvents(databaseContext databaseContext, List<EncodeJob> waitingJobs)
    {
        foreach (EncodeJob encodeJob in waitingJobs)
        {
            if (!maxThreadsReached())
            {
                onEncodeJobFoundInQueue(new QueueScanItemFoundEventArgs(encodeJob));
            }
            else
            {
                break;
            }
        }
    }

    private void onEncodeJobFoundInQueue(QueueScanItemFoundEventArgs e)
    {
        SampleEventHandler handler = encodeJobFoundInQueue;
        handler?.Invoke(this, e);
    }

    public async Task forceScan(databaseContext databaseContext)
    {
        Logger.v("Forcing queue scan...");

        await this.scan(databaseContext);
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