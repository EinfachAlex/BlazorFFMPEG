using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Jobs;
using BlazorFFMPEG.Backend.Modules.Logging;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlazorFFMPEG.Backend.Controllers.Post
{
    public class StartEncodeController : ControllerBase
    {
        private const string ENDPOINT = "/startEncode";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.POST;

        private readonly databaseContext _context;
        private readonly ILogger _logger;
        private readonly QueueScannerJob _queueScannerJob;

        public StartEncodeController(databaseContext context, ILogger<StartEncodeController> logger, QueueScannerJob queueScannerJob)
        {
            _context = context;
            _logger = logger;
            _queueScannerJob = queueScannerJob;
        }

        [HttpPost(ENDPOINT)]
        public async Task<ObjectResult> PostStartEncode(
            [FromForm, BindRequired] string codec,
            [FromForm, BindRequired] string inputFile, 
            [FromForm, BindRequired] string qualityMethod, 
            [FromForm, BindRequired] long qualityValue)
        {
            if (!ModelState.IsValid)
            {
                _logger.logParametersMissing();
                return BadRequest(ModelState);
            }

            EncodeJob createdEncodeJob = EncodeJob.constructNew(_context, codec, inputFile, qualityMethod, qualityValue, commit: true);

            await _queueScannerJob.forceScan(_context);
            
            return Ok(JsonSerializer.Serialize(createdEncodeJob, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles }));
        }
    }
}