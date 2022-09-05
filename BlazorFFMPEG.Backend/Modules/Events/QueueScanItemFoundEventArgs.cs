using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Jobs;

namespace BlazorFFMPEG.Backend.Modules.Events;

public class QueueScanItemFoundEventArgs : EventArgs
{
    public EncodeJob job { get; }
    
    public QueueScanItemFoundEventArgs(EncodeJob encodeJob)
    {
        this.job = encodeJob;
    }
}