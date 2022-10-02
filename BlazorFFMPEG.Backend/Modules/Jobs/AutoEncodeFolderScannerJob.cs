using System.Net.WebSockets;
using System.Text;
using BlazorFFMPEG.Backend.Controllers.Get;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Events;
using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.Threads;

namespace BlazorFFMPEG.Backend.Modules.Jobs;

/**
 * Job that scans auto encode folders for new files and adds them to the queue (<see cref="BlazorFFMPEG.Backend.Modules.Jobs.QueueScannerJob"/>
 * (Table: auto_encode_folder), gets started by <see cref="JobManager"/>
 */
public class AutoEncodeFolderScannerJob
{
    private readonly databaseContext _context;
    private readonly ILogger _logger;
    private readonly FFMPEG.FFMPEG _ffmpeg;
    private readonly IServiceProvider _serviceProvider;

    public AutoEncodeFolderScannerJob(ILogger<QueueScannerJob> logger, FFMPEG.FFMPEG ffmpeg, IServiceProvider services)
    {
        //_context = context;
        _logger = logger;
        _ffmpeg = ffmpeg;
        _serviceProvider = services;
    }

    //Raised for every encode job that is found in the queue
    // public delegate void SampleEventHandler(object sender, QueueScanItemFoundEventArgs e);
    // public event SampleEventHandler encodeJobFoundInQueue;

    private bool scanningEnabled = false;

    public void startScanning(string connectionString)
    {
        using (databaseContext databaseContext = new databaseContext(connectionString))
        {
            scanningEnabled = true;

            SetIntervalUtil.SetInterval(async () =>
                {
                    //Needed because variable in outer scope will get disposed
                    using (databaseContext databaseContext = new databaseContext())
                    {
                        await this.scan(databaseContext);
                    }
                },
                TimeSpan.FromMinutes(1));
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
        List<AutoEncodeFolder> autoEncodeFolders = await getAutoEncodeFolders(databaseContext);

        byte[] websocketMessage = Encoding.ASCII.GetBytes($"Scanned for auto-encode folders, found {autoEncodeFolders.Count} folders");
        WebSocketController.websocketServer?.SendAsync(new ArraySegment<byte>(websocketMessage, 0, websocketMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);

        //SCAN FOLDERS RAISE EVENTS FOR EVERY FILE
        foreach (AutoEncodeFolder autoEncodeFolder in autoEncodeFolders)
        {
            if (Directory.Exists(autoEncodeFolder.Inputpath))
            {
                IEnumerable<string> enumerateFiles = Directory.EnumerateFiles(autoEncodeFolder.Inputpath);
            }
            ;

        }

        //SAVE SCANNED FILES IN DATABASE TABLE
        //raiseEvents(databaseContext, autoEncodeFolders);
    }

    public async Task<List<AutoEncodeFolder>> getAutoEncodeFolders(databaseContext databaseContext)
    {
        List<AutoEncodeFolder> autoEncodeFolders = databaseContext.AutoEncodeFolders.ToList();

        return autoEncodeFolders;
    }

    private void raiseEvents(databaseContext databaseContext, List<EncodeJob> waitingJobs)
    {

    }
}