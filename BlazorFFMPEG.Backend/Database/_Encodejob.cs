using BlazorFFMPEG.Backend.Modules.Jobs;
using EinfachAlex.Utils.HashGenerator;
using EinfachAlex.Utils.Logging;
using Microsoft.EntityFrameworkCore;

namespace BlazorFFMPEG.Backend.Database;

public partial class EncodeJob
{
    public static EncodeJob constructNew(databaseContext databaseContext, string codec, string file, bool commit = false)
    {
        LoggerCommonMessages.logConstructNew(file);

        Hash id = generateId(codec, file);

        EncodeJob proxyObject = new EncodeJob()
        {
            Codec = codec,
            Path = file,
            Status = (int)EEncodingStatus.NEW
        };

        var proxy = databaseContext.CreateProxy<EncodeJob>();
        databaseContext.Entry(proxy).CurrentValues.SetValues(proxyObject);

        databaseContext.Add(proxy);

        if (commit) databaseContext.SaveChanges();

        return proxy;
    }

    public static Hash generateId(string codec, string file)
    {
        string key = $"{codec}{file}";

        Hash id = HashGenerator.generateSHA256(key);
        LoggerCommonMessages.logGeneratedId(id);

        return id;
    }
    public void setStatus(EEncodingStatus working)
    {
        Logger.i($"Changing status of {Jobid} to {working}");
        
        this.Status = (int)working;
    }
}