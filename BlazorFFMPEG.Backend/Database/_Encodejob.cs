using System.Net.WebSockets;
using System.Text;
using BlazorFFMPEG.Backend.Controllers.Get;
using BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;
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
    
    public static EncodeJob constructNew(databaseContext databaseContext, EncoderBase encoderBase, ConstantsQualitymethod qualityMethodObject, long qualityValue, string inputFile, bool commit)
    {
        LoggerCommonMessages.logConstructNew(inputFile);

        Hash id = generateId(encoderBase, qualityMethodObject, (int)qualityValue, inputFile);

        EncodeJob proxyObject = new EncodeJob()
        {
            Codec = encoderBase.ToString(),
            Path = inputFile,
            Status = (int)EEncodingStatus.NEW,
            Qualitymethod = qualityMethodObject.Id,
            Qualityvalue = (int)qualityValue
        };

        var proxy = databaseContext.CreateProxy<EncodeJob>();
        databaseContext.Entry(proxy).CurrentValues.SetValues(proxyObject);

        databaseContext.Add(proxy);

        if (commit) databaseContext.SaveChanges();

        return proxy;
    }
    
    private static Hash generateId(EncoderBase encoderBase, ConstantsQualitymethod qualityMethodObject, int qualityValue, string inputFile)
    {
        string key = $"{encoderBase}{qualityMethodObject.getQualityMethodAsEnum()}{qualityValue}{inputFile}";

        Hash id = HashGenerator.generateSHA256(key);
        LoggerCommonMessages.logGeneratedId(id);

        return id;
    }

    public static Hash generateId(ReadOnlySpan<char> codec, ReadOnlySpan<char> file)
    {
        string key = $"{codec}{file}";

        Hash id = HashGenerator.generateSHA256(key);
        LoggerCommonMessages.logGeneratedId(id);

        return id;
    }
    
    public void setStatus(databaseContext databaseContext, EEncodingStatus newStatus)
    {
        Logger.i($"Changing status of {Jobid} from {this.StatusNavigation.Description} to {newStatus}");

        byte[] websocketMessage = Encoding.ASCII.GetBytes($"Job {Jobid} is now in status {newStatus.ToString()}");
        WebSocketController.websocketServer?.SendAsync(new ArraySegment<byte>(websocketMessage, 0, websocketMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);

        this.Status = (int)newStatus;
        
        databaseContext.Entry(this).Reload();
    }

    public void resetStatus(databaseContext databaseContext)
    {
        this.setStatus(databaseContext, EEncodingStatus.NEW);
    }
    

}