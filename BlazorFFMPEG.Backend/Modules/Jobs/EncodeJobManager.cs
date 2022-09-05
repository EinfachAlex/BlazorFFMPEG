using BlazorFFMPEG.Backend.Database;

namespace BlazorFFMPEG.Backend.Modules.Jobs;

public class EncodeJobManager
{
    private static EncodeJobManager? instance;

    public static EncodeJobManager? getInstance()
    {
        return instance ??= new EncodeJobManager();
    }

    public Task addEncodeJob(databaseContext databaseContext, string codec, string file)
    {
        EncodeJob.constructNew(databaseContext, false, codec, file);

        return Task.CompletedTask;
    }
}