using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using BlazorFFMPEG.Backend.Controllers.Get;
using EinfachAlex.Utils.Logging;
using Encoder = BlazorFFMPEG.Shared.DTO.Encoder;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG;

public class FFMPEG
{
    public async Task startEncode(string inputFile, string codec, int key)
    {
        Process ffmpegProcess = new Process
        {
            StartInfo =
            {
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        ffmpegProcess.OutputDataReceived += (_, args) => Console.WriteLine(args.Data);
        ffmpegProcess.ErrorDataReceived += (_, args) => Console.WriteLine(args.Data);

        ffmpegProcess.StartInfo.FileName = "ffmpeg";
        ffmpegProcess.StartInfo.Arguments = $" -i {inputFile} -c:v {codec} out{key}.mp4 -y";

        ffmpegProcess.Start();

        byte[] websocketMessage = Encoding.ASCII.GetBytes($"Starting encoding {inputFile} (Job {key}) to {codec}");
        WebSocketController.websocketServer?.SendAsync(new ArraySegment<byte>(websocketMessage, 0, websocketMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
        
        ffmpegProcess.BeginErrorReadLine();
        ffmpegProcess.BeginOutputReadLine();

        ffmpegProcess.WaitForExit();

        var websocketMessageEncodingFinished = Encoding.ASCII.GetBytes($"Encoding job {key} finished.");
        WebSocketController.websocketServer?.SendAsync(
            new ArraySegment<byte>(websocketMessageEncodingFinished, 0, websocketMessageEncodingFinished.Length),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }

    public async Task<List<Encoder>> getAvailableEncoders()
    {
        List<Encoder> availableEncoders = new List<Encoder>();

        Process ffmpegProcess = new Process
        {
            StartInfo =
            {
                CreateNoWindow = true,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        ffmpegProcess.OutputDataReceived += (_, args) =>
        {
            try
            {
                Encoder codec = new Encoder(args.Data.Split(' ')[2].Split(' ')[0]);

                if (codec.name == "=") return;

                Logger.v($"Encoder {codec.name} found");

                availableEncoders.Add(codec);
            }
            catch (Exception e)
            {
                Logger.v($"No encoder could be parsed for {args.Data}");
            }
        };

        ffmpegProcess.StartInfo.FileName = "ffmpeg";
        ffmpegProcess.StartInfo.Arguments = "-encoders";

        ffmpegProcess.Start();

        ffmpegProcess.BeginErrorReadLine();
        ffmpegProcess.BeginOutputReadLine();
        await ffmpegProcess.WaitForExitAsync();

        return availableEncoders;
    }
}