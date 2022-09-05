using System.Diagnostics;
using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlazorFFMPEG.Backend.Controllers.Get
{
    public class GetHelloWorldController : ControllerBase
    {
        private const string ENDPOINT = "/helloWorld";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.GET;

        [HttpGet(ENDPOINT)]
        public string GetHelloWorld()
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoggerCommonMessages.logEndpointRequest(ENDPOINT, ENDPOINT_TYPE);

            string response = "Hello World!";
            
            sw.Stop();
            Logger.v($"{sw.ElapsedMilliseconds} ms");

            return response;
        }
    }
}