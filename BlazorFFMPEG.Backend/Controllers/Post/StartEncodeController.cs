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
        public ObjectResult PostStartEncode([FromForm] string codec, [FromForm] string inputFile)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoggerCommonMessages.logEndpointRequest(ENDPOINT, ENDPOINT_TYPE);

            if (codec == null || inputFile == null) return Problem("Du hast reingeschissen!");

            EncodeJob createdEncodeJob;
            using (databaseContext databaseContext = new databaseContext())
            {
                createdEncodeJob = EncodeJob.constructNew(databaseContext, codec, inputFile, commit: true);
                
                sw.Stop();
                Logger.v($"{sw.ElapsedMilliseconds} ms");
                ServerLoadAnalyzer.getInstance().handleLoad(sw.ElapsedMilliseconds);

                return Ok(JsonSerializer.Serialize(createdEncodeJob, new JsonSerializerOptions(){ ReferenceHandler = ReferenceHandler.IgnoreCycles}));
            }
        }
    }
}