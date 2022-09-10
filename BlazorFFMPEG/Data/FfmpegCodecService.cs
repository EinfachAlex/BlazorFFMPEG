using System.Diagnostics;
using System.Text.Json;
using BlazorFFMPEG.Shared.Constants;
using BlazorFFMPEG.Shared.DTO;
using RestSharp;

namespace BlazorFFMPEG.Data
{
    public class FfmpegCodecService
    {
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

            List<Encoder> popularCodecs = availableCodecs.FindAll(x => x.name.ToUpper() == EEncoders.LIBX264
                                                                       || x.name.ToUpper() == EEncoders.HEVC_NVENC);
            
            foreach (Encoder popularCodec in popularCodecs)
            {
                availableCodecs.Remove(popularCodec);
                availableCodecs.Insert(0, popularCodec);
            }

            return availableCodecs;
        }
        
        public async Task<string?> startEncode(string filePath, Encoder encoder)
        {
            var client = new RestClient("https://localhost:7208/");
            var request = new RestRequest("startEncode", Method.Post);
            
            request.AlwaysMultipartFormData = true;
            request.AddParameter("codec", encoder.name);
            request.AddParameter("inputFile", filePath);
            
            var response = await client.PostAsync(request);

            return response.Content;
        }
        
        public async Task<List<AvailableQualityMethod>> getAvailableQualityMethods()
        {
            var client = new RestClient("https://localhost:7208/");
            var request = new RestRequest("getAvailableQualityMethods", Method.Get);
            
            var response = await client.GetAsync(request);
            Console.WriteLine(response.Content);

            List<AvailableQualityMethod> encoders = JsonSerializer.Deserialize<List<AvailableQualityMethod>>(response.Content);
            
            return encoders;
        }
    }
}