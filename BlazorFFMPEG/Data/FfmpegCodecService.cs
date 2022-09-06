using System.Diagnostics;
using System.Text.Json;
using BlazorFFMPEG.Shared.DTO;
using RestSharp;

namespace BlazorFFMPEG.Data
{
    public class FfmpegCodecService
    {
        const string CODEC_H264 = "LIBX264";
        const string CODEC_HEVC_NVENC = "HEVC_NVENC";

        public async Task<List<Encoder>> getAvailableCodecs()
        {
            var client = new RestClient("https://localhost:7208/");
            var request = new RestRequest("getAvailableEncoders", Method.Get);
            
            var response = await client.GetAsync(request);
            Console.WriteLine(response.Content);

            List<Encoder> encoders = JsonSerializer.Deserialize<List<Encoder>>(response.Content);
            
            return encoders;
        }

        public async Task<List<Encoder>> getAvailableCodecs_WithCustomSort()
        {
            List<Encoder> availableCodecs = await getAvailableCodecs();

            List<Encoder> popularCodecs = availableCodecs.FindAll(x => x.name.ToUpper() == CODEC_H264
                                                                       || x.name.ToUpper() == CODEC_HEVC_NVENC);
            
            foreach (Encoder popularCodec in popularCodecs)
            {
                availableCodecs.Remove(popularCodec);
                availableCodecs.Insert(0, popularCodec);
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