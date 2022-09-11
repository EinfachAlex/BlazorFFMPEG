using System.Net.WebSockets;
using System.Text;
using BlazorFFMPEG.Backend.Controllers.Get;
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
    public void setStatus(databaseContext databaseContext, EEncodingStatus newStatus)
    {
        Logger.i($"Changing status of {Jobid} from {this.StatusNavigation.Description} to {newStatus}");

        byte[] websocketMessage = Encoding.ASCII.GetBytes($"Job {Jobid} is now in status {newStatus.ToString()}");
        WebSocketController.websocketServer?.SendAsync(new ArraySegment<byte>(websocketMessage, 0, websocketMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);

        this.Status = (int)newStatus;
    }

    public void resetStatus(databaseContext databaseContext)
    {
        this.setStatus(databaseContext, EEncodingStatus.NEW);
    }
}