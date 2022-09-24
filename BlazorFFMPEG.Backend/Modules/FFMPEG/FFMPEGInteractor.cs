using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using BlazorFFMPEG.Backend.Controllers.Get;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;
using BlazorFFMPEG.Backend.Modules.Logging;
using BlazorFFMPEG.Shared.Constants;
using EinfachAlex.Utils.Logging;
using FFMpegCore;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG;

public class FFMPEG
{
    private readonly ILogger _logger;

    public FFMPEG(ILogger<FFMPEG> logger)
    {
        _logger = logger;
    }
    
    public async Task startEncode(string inputFile, string encoder, ConstantsQualitymethod qualityMethod, int key)
    {
        await FFMpegArguments
            .FromFileInput(inputFile)
            .OutputToFile($"out{key}.mp4", true, options => options
                .WithVideoCodec(encoder.ToLower()))
            .NotifyOnProgress(d =>
            {
                Console.WriteLine(d.ToString());
            })
            .NotifyOnOutput(s =>
            {
                Console.WriteLine(s);
            })
            .ProcessAsynchronously();
        
        /*Process ffmpegProcess = new Process
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
        ffmpegProcess.StartInfo.Arguments = buildFFMPEGArguments(inputFile, encoder, key);

        ffmpegProcess.Start();

          
        ffmpegProcess.BeginErrorReadLine();
        ffmpegProcess.BeginOutputReadLine();

        ffmpegProcess.WaitForExit();*/
        
        byte[] websocketMessage = Encoding.ASCII.GetBytes($"Starting encoding {inputFile} (Job {key}) to {encoder.ToString()}");
        WebSocketController.websocketServer?.SendAsync(new ArraySegment<byte>(websocketMessage, 0, websocketMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);
        
        var websocketMessageEncodingFinished = Encoding.ASCII.GetBytes($"Encoding job {key} finished.");
        WebSocketController.websocketServer?.SendAsync(
            new ArraySegment<byte>(websocketMessageEncodingFinished, 0, websocketMessageEncodingFinished.Length),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
    }
    private static string buildFFMPEGArguments(ReadOnlySpan<char> inputFile, EncoderBase encoder, int key)
    {
        string arguments = "";

        arguments += addInput(inputFile);

        string addInput(ReadOnlySpan<char> inputFile)
        {
            return "-i {inputFile}";
        }

        switch (encoder.getAsEnum())
        {

            case EEncoders.LIBX264:
                break;

            case EEncoders.HEVC_NVENC:
                break;

            case EEncoders.LIBSVTAV1:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        return $" -i {inputFile} -c:v {encoder.ToString()} out{key}.mp4 -y";
    }

    public async Task<List<Shared.DTO.EncoderDTO>> getAvailableEncoders()
    {
        List<Shared.DTO.EncoderDTO> availableEncoders = new List<Shared.DTO.EncoderDTO>();

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
                Shared.DTO.EncoderDTO codec = new Shared.DTO.EncoderDTO(args.Data.Split(' ')[2].Split(' ')[0]);

                if (codec.name == "=") return;
                
                _logger.logEncoderFound(codec.name);

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