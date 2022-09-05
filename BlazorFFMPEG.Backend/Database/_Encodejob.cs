using BlazorFFMPEG.Backend.Modules.Jobs;
using EinfachAlex.Utils.HashGenerator;
using EinfachAlex.Utils.Logging;
using Microsoft.EntityFrameworkCore;

namespace BlazorFFMPEG.Backend.Database;

public partial class EncodeJob
{
    public static EncodeJob constructNew(databaseContext databaseContext, bool commit, string codec, string file)
    {
        LoggerCommonMessages.logConstructNew(file);

        Hash id = generateId(codec, file);

        EncodeJob proxyObject = new EncodeJob()
        {
            Codec = codec,
            Path = file
        };

        var proxy = databaseContext.CreateProxy<EncodeJob>();
        databaseContext.Entry(proxy).CurrentValues.SetValues(proxyObject);

        databaseContext.Add(proxy);

        if(commit) databaseContext.SaveChanges();

        return proxy;
    }
    
    private static Hash generateId(string codec, string file)
    {
        string key = $"{codec}{file}";

        Hash id = HashGenerator.generateSHA256(key);
        LoggerCommonMessages.logGeneratedId(id);

        return id;
    }
}