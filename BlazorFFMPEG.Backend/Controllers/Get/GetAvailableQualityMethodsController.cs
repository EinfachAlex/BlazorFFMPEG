using System.Diagnostics;
using BlazorFFMPEG.Backend.Modules.FFMPEG;
using BlazorFFMPEG.Shared.Constants;
using BlazorFFMPEG.Shared.DTO;
using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlazorFFMPEG.Backend.Controllers.Get
{
    public class GetAvailableQualityMethodsController : ControllerBase
    {
        private const string ENDPOINT = "/getAvailableQualityMethods";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.GET;

        [HttpGet(ENDPOINT)]
        public async Task<List<AvailableQualityMethod>> GetAvailableQualityMethods()
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoggerCommonMessages.logEndpointRequest(ENDPOINT, ENDPOINT_TYPE);

            List<AvailableQualityMethod> availableQualityMethods = new List<AvailableQualityMethod>()
            {
                new AvailableQualityMethod(EQualityMethods.Bitrate.ToString()),
                new AvailableQualityMethod(EQualityMethods.CQ.ToString()),
            };

            sw.Stop();
            Logger.v($"{sw.ElapsedMilliseconds} ms");

            return availableQualityMethods;
        }
    }
}