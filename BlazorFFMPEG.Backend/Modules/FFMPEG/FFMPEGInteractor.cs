using System.Diagnostics;

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
        ffmpegProcess.StartInfo.Arguments = $" -i {inputFile} -c:v {codec} out.mp4";

        ffmpegProcess.Start();

        ffmpegProcess.BeginErrorReadLine();
        ffmpegProcess.BeginOutputReadLine();
    }
}