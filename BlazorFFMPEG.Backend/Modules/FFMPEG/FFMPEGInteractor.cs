using System.Diagnostics;
using BlazorFFMPEG.Shared.DTO;
using EinfachAlex.Utils.Logging;

namespace BlazorFFMPEG.Backend.Modules.FFMPEG;

public class FFMPEG
{
    public async Task startEncode(string inputFile, string codec)
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
        ffmpegProcess.StartInfo.Arguments = $" -i {inputFile} -c:v {codec} out.mp4 -y";

        ffmpegProcess.Start();

        ffmpegProcess.BeginErrorReadLine();
        ffmpegProcess.BeginOutputReadLine();
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