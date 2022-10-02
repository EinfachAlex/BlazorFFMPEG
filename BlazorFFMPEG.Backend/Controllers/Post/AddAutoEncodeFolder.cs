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
    public class AddAutoEncodeFolderController : ControllerBase
    {
        private const string ENDPOINT = "/addAutoEncodeFolder";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.POST;

        private readonly databaseContext _context;
        private readonly ILogger _logger;
        private readonly QueueScannerJob _queueScannerJob;

        public AddAutoEncodeFolderController(databaseContext context, ILogger<StartEncodeController> logger, QueueScannerJob queueScannerJob)
        {
            _context = context;
            _logger = logger;
            _queueScannerJob = queueScannerJob;
        }

        [HttpPost(ENDPOINT)]
        public async Task<ObjectResult> PostAddAutoEncodeFolder(
            [FromForm, BindRequired] string codec,
            [FromForm, BindRequired] string inputFolder, 
            [FromForm, BindRequired] string outputFolder, 
            [FromForm, BindRequired] string qualityMethod, 
            [FromForm, BindRequired] long qualityValue)
        {
            if (!ModelState.IsValid)
            {
                _logger.logParametersMissing();
                return BadRequest(ModelState);
            }

            AutoEncodeFolder autoEncodeFolder = AutoEncodeFolder.constructNew(_context, codec, inputFolder, outputFolder, qualityMethod, qualityValue, commit: true);
            
            return Ok(JsonSerializer.Serialize(autoEncodeFolder, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles }));
        }
    }
}