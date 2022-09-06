using System.Diagnostics;
using BlazorFFMPEG.Backend.Modules.FFMPEG;
using BlazorFFMPEG.Shared.DTO;
using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlazorFFMPEG.Backend.Controllers.Get
{
    public class GetAvailableEncodersController : ControllerBase
    {
        private const string ENDPOINT = "/getAvailableEncoders";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.GET;

        [HttpGet(ENDPOINT)]
        public async Task<List<Encoder>> GetAvailableEncoders()
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoggerCommonMessages.logEndpointRequest(ENDPOINT, ENDPOINT_TYPE);

            List<Encoder> availableEncodersFromFfmpeg = await new FFMPEG().getAvailableEncoders();

            sw.Stop();
            Logger.v($"{sw.ElapsedMilliseconds} ms");

            return availableEncodersFromFfmpeg;
        }
    }
}