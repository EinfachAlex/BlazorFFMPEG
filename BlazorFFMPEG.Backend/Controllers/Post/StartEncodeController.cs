using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.FFMPEG.Encoder;
using BlazorFFMPEG.Backend.Modules.FFMPEG.QualityMethods;
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

        private readonly databaseContext _context;

        public StartEncodeController(databaseContext context)
        {
            _context = context;
        }
        
        [HttpPost(ENDPOINT)]
        public async Task<ObjectResult> PostStartEncode([FromForm] string codec, [FromForm] string inputFile, [FromForm] string qualityMethod, [FromForm] string qualityValue)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoggerCommonMessages.logEndpointRequest(ENDPOINT, ENDPOINT_TYPE);

            if (!checkParametersFilled(codec, inputFile, qualityMethod, qualityValue)) return Problem("Du hast reingeschissen!");

            using (databaseContext databaseContext = new databaseContext())
            {
                EncodeJob createdEncodeJob;

                Encoder encoder = Encoder.constructByString(codec);
                encoder.checkQualityMethodIsCompatibleWithEncoder(qualityMethod, out QualityMethod qualityMethodObject);
                encoder.checkQualityMethodValue(qualityMethodObject, qualityValue);

                long qualityValueLong = Convert.ToInt64(qualityValue);
                
                createdEncodeJob = EncodeJob.constructNew(databaseContext, encoder, qualityMethodObject, qualityValueLong, inputFile, commit: true);

                await QueueScannerJob.getInstance().forceScan(databaseContext);
                
                sw.Stop();
                Logger.v($"{sw.ElapsedMilliseconds} ms");
                ServerLoadAnalyzer.getInstance().handleLoad(sw.ElapsedMilliseconds);

                return Ok(JsonSerializer.Serialize(createdEncodeJob, new JsonSerializerOptions(){ ReferenceHandler = ReferenceHandler.IgnoreCycles}));
            }
        }
        private bool checkParametersFilled(string codec, string inputFile, string qualityMethod, string qualityValue)
        {
            return !(String.IsNullOrEmpty(codec)
                    || String.IsNullOrEmpty(inputFile)
                    || String.IsNullOrEmpty(qualityMethod)
                    || String.IsNullOrEmpty(qualityValue));
        }
    }
}