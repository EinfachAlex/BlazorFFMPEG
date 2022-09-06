using System.Diagnostics;

namespace BlazorFFMPEG.Data
{
    public class FfmpegCodecService
    {
        const string CODEC_H264 = "LIBX264";
        const string CODEC_HEVC_NVENC = "HEVC_NVENC";

        public async Task<List<FfmpegCodec>> getAvailableCodecs()
        {
            List<FfmpegCodec> availableCodecs = new List<FfmpegCodec>();

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
                    
                    FfmpegCodec codec = new FfmpegCodec(args.Data.Split(' ')[2].Split(' ')[0]);

                    if (codec.name == "=") return;
                    
                    availableCodecs.Add(codec);
                }
                catch (Exception e)
                {
                }
            };

            ffmpegProcess.StartInfo.FileName = "ffmpeg";
            ffmpegProcess.StartInfo.Arguments = "-encoders";

            ffmpegProcess.Start();

            ffmpegProcess.BeginErrorReadLine();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.WaitForExit();

            return availableCodecs;
        }

        public async Task<List<FfmpegCodec>> getAvailableCodecs_WithCustomSort()
        {
            List<FfmpegCodec> availableCodecs = await getAvailableCodecs();

            List<FfmpegCodec> popularCodecs = availableCodecs.FindAll(x => x.name.ToUpper() == CODEC_H264
                                                                           || x.name.ToUpper() == CODEC_HEVC_NVENC);
            
            foreach (FfmpegCodec popularCodec in popularCodecs)
            {
                availableCodecs.Remove(popularCodec);
                availableCodecs.Insert(1, popularCodec);
            }

            return availableCodecs;
        }
        
        public void startEncode(string filePath, string selectedCodec)
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
            ffmpegProcess.StartInfo.Arguments = $" -i {filePath} -c:v {selectedCodec} out.mp4";

            ffmpegProcess.Start();

            ffmpegProcess.BeginErrorReadLine();
            ffmpegProcess.BeginOutputReadLine();
            ffmpegProcess.WaitForExit();
        }
    }
}