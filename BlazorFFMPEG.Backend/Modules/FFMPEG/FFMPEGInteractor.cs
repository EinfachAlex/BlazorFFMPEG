using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using BlazorFFMPEG.Backend.Controllers.Get;
using BlazorFFMPEG.Backend.Modules.Logging;
using BlazorFFMPEG.Shared.Constants;
using FFMpegCore;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG;

public class FFMPEG
{
    private readonly ILogger _logger;

    public FFMPEG(ILogger<FFMPEG> logger)
    {
        _logger = logger;
    }
    
    public async Task startEncode(string inputFile, string encoder, string qualityMethod, int qualityValue, int key)
    {
        Enum.TryParse(qualityMethod, out EQualityMethods qualityMethodAsEnum);
        
        FFMpegArgumentProcessor ffmpegCommand = FFMpegArguments
            .FromFileInput(inputFile)
            .OutputToFile($"out{key}.mp4", true, options =>
            {
                FFMpegArgumentOptions optionsWithQualityValue = qualityMethodAsEnum switch
                {
                    EQualityMethods.Bitrate => options.WithVideoBitrate(qualityValue),
                    EQualityMethods.CQ => options.WithConstantRateFactor(qualityValue),
                    _ => throw new ArgumentOutOfRangeException()
                };

                optionsWithQualityValue.WithVideoCodec(encoder.ToLower());
            });

        Task<bool> ffmpegTask = ffmpegCommand
            .NotifyOnProgress(d =>
            {
                Console.WriteLine(d.ToString());
            })
            .NotifyOnOutput(s =>
            {
                Console.WriteLine(s);
            })
            .NotifyOnProgress(o =>
            {
                Console.WriteLine(o.ToString());
            })
            .ProcessAsynchronously();

        byte[] websocketMessage = Encoding.ASCII.GetBytes($"Starting encoding {inputFile} (Job {key}) to {encoder.ToString()}");
        WebSocketController.websocketServer?.SendAsync(new ArraySegment<byte>(websocketMessage, 0, websocketMessage.Length), WebSocketMessageType.Text, WebSocketMessageFlags.EndOfMessage, CancellationToken.None);

        await ffmpegTask;
        
        var websocketMessageEncodingFinished = Encoding.ASCII.GetBytes($"Encoding job {key} finished.");
        WebSocketController.websocketServer?.SendAsync(
            new ArraySegment<byte>(websocketMessageEncodingFinished, 0, websocketMessageEncodingFinished.Length),
            WebSocketMessageType.Text,
            WebSocketMessageFlags.EndOfMessage,
            CancellationToken.None);
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
                //Logger.v($"No encoder could be parsed for {args.Data}");
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