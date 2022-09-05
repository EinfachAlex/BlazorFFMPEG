﻿using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Events;

namespace BlazorFFMPEG.Backend.Modules.Jobs;

public class EncodeJobManager
{
    private static EncodeJobManager? instance;

    public static EncodeJobManager? getInstance()
    {
        return instance ??= new EncodeJobManager();
    }

    public EncodeJobManager()
    {
        QueueScannerJob.getInstance().encodeJobFoundInQueue += encodeJobFoundInQueue;
    }
    private void encodeJobFoundInQueue(object? sender, QueueScanItemFoundEventArgs e)
    {
        new FFMPEG.FFMPEG().startEncode(e.job.Path, e.job.Codec);
    }
    
    public Task addEncodeJob(databaseContext databaseContext, string codec, string file)
    {
        EncodeJob.constructNew(databaseContext, false, codec, file);

        return Task.CompletedTask;
    }
}