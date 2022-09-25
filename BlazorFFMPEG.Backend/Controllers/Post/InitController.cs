using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.Jobs;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;

namespace BlazorFFMPEG.Backend.Controllers.Post
{
    public class InitController : ControllerBase
    {
        private const string ENDPOINT = "/init";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.POST;

        private readonly databaseContext _context;
        private readonly ILogger _logger;
        private readonly JobManager _jobManager;

        public InitController(databaseContext context, ILogger<StartEncodeController> logger, JobManager jobManager)
        {
            _context = context;
            _logger = logger;
            _jobManager = jobManager;
        }

        [HttpPost(ENDPOINT)]
        public async Task<ObjectResult> PostInit()
        {
            _jobManager.startJobThreads(_context);

            return Ok("Ok!");
        }
    }
}