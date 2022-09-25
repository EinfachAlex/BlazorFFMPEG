using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Jobs;
using BlazorFFMPEG.Backend.Modules.Logging;
using BlazorFFMPEG.Backend.Modules.ServerLoad;
using EinfachAlex.Utils.Logging;
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



        public StartEncodeController(databaseContext context, ILogger<StartEncodeController> logger)
        {
            _context = context;
            _logger = logger;
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
            
            EncoderBase encoderBase = EncoderBase.constructByString(codec);
            encoderBase.checkQualityMethodIsCompatibleWithEncoder(_context, qualityMethod, out ConstantsQualitymethod qualityMethodObject);
            encoderBase.checkQualityMethodValue(_context, qualityMethodObject, qualityValue);

            long qualityValueLong = Convert.ToInt64(qualityValue);

            EncodeJob createdEncodeJob = EncodeJob.constructNew(_context, encoderBase, qualityMethodObject, qualityValueLong, inputFile, commit: true);

            await QueueScannerJob.getInstance().forceScan(_context);

            return Ok(JsonSerializer.Serialize(createdEncodeJob, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles }));
        }
    }
}