using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Jobs;
using BlazorFFMPEG.Backend.Modules.ServerLoad;
using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFFMPEG.Backend.Controllers.Post
{
    public class StartEncodeController : ControllerBase
    {
        private const string ENDPOINT = "/startEncode";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.POST;

        [HttpPost(ENDPOINT)]
        public async Task<ObjectResult> PostStartEncode([FromForm] string codec, [FromForm] string inputFile, [FromForm] string qualityMethod, [FromForm] string qualityValue)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoggerCommonMessages.logEndpointRequest(ENDPOINT, ENDPOINT_TYPE);

            if (!checkParametersFilled(codec, inputFile, qualityMethod, qualityValue)) return Problem("Du hast reingeschissen!");

            EncodeJob createdEncodeJob;
            using (databaseContext databaseContext = new databaseContext())
            {
                createdEncodeJob = EncodeJob.constructNew(databaseContext, codec, inputFile, commit: true);

                await QueueScannerJob.getInstance().forceScan(databaseContext);
                
                sw.Stop();
                Logger.v($"{sw.ElapsedMilliseconds} ms");
                ServerLoadAnalyzer.getInstance().handleLoad(sw.ElapsedMilliseconds);

                return Ok(JsonSerializer.Serialize(createdEncodeJob, new JsonSerializerOptions(){ ReferenceHandler = ReferenceHandler.IgnoreCycles}));
            }
        }
    }
}