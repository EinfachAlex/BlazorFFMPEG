using System.Diagnostics;
using System.Text.Json;
using BlazorFFMPEG.Shared.Constants;
using BlazorFFMPEG.Shared.DTO;
using RestSharp;

namespace BlazorFFMPEG.Data
{
    public class FfmpegCodecService
    {
        public async Task<List<EncoderDTO>> getAvailableCodecs()
        {
            var client = new RestClient("https://localhost:7208/");
            var request = new RestRequest("getAvailableEncoders", Method.Get);
            
            var response = await client.GetAsync(request);
            Console.WriteLine(response.Content);

            List<EncoderDTO> encoders = JsonSerializer.Deserialize<List<EncoderDTO>>(response.Content);
            
            return encoders;
        }

        public async Task<List<EncoderDTO>> getAvailableCodecs_WithCustomSort()
        {
            List<EncoderDTO> availableCodecs = await getAvailableCodecs();

            List<EncoderDTO> popularCodecs = availableCodecs.FindAll(x => x.name.ToUpper() == EEncoders.LIBX264.ToString()
                                                                       || x.name.ToUpper() == EEncoders.HEVC_NVENC.ToString());
            
            foreach (EncoderDTO popularCodec in popularCodecs)
            {
                availableCodecs.Remove(popularCodec);
                availableCodecs.Insert(0, popularCodec);
            }

            return availableCodecs;
        }
        
        public async Task<string?> startEncode(AddEncodeJobModel addEncodeJobModel)
        {
            var client = new RestClient("https://localhost:7208/");
            var request = new RestRequest("startEncode", Method.Post);
            
            request.AlwaysMultipartFormData = true;
            request.AddParameter("codec", addEncodeJobModel.encoder.name);
            request.AddParameter("inputFile", addEncodeJobModel.filePath);
            request.AddParameter("qualityMethod", addEncodeJobModel.selectedQualityMethod.name);
            request.AddParameter("qualityValue", addEncodeJobModel.qualityValue);
            
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