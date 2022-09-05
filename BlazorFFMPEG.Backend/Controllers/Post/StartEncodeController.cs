using System.Diagnostics;
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
        public ContentResult PostStartEncode([FromForm] string codec, [FromForm] string inputFile)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoggerCommonMessages.logEndpointRequest(ENDPOINT, ENDPOINT_TYPE);

            if (codec == null || inputFile == null) return Content("No parameters given");
            
            using (databaseContext databaseContext = new databaseContext())
            {
                EncodeJobManager.getInstance()!.addEncodeJob(databaseContext, codec, inputFile);
                databaseContext.SaveChanges();
            }

            sw.Stop();
            Logger.v($"{sw.ElapsedMilliseconds} ms");
            ServerLoadAnalyzer.getInstance().handleLoad(sw.ElapsedMilliseconds);

            return Content("JSON");
        }
    }
}