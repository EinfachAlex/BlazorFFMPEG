using System.Diagnostics;
using BlazorFFMPEG.Backend.Database;
using BlazorFFMPEG.Backend.Modules.FFMPEG;
using BlazorFFMPEG.Backend.Modules.Logging;
using BlazorFFMPEG.Shared.DTO;
using EinfachAlex.Utils.Logging;
using EinfachAlex.Utils.WebRequest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlazorFFMPEG.Backend.Controllers.Get
{
    public class GetAvailableEncodersController : ControllerBase
    {
        private const string ENDPOINT = "/getAvailableEncoders";
        private const ERequestTypes ENDPOINT_TYPE = ERequestTypes.GET;

        private readonly databaseContext _context;
        private readonly ILogger _logger;
        private readonly FFMPEG _ffmpeg;

        public GetAvailableEncodersController(databaseContext context, ILogger<GetAvailableEncodersController> logger, FFMPEG ffmpeg)
        {
            _context = context;
            _logger = logger;
            _ffmpeg = ffmpeg;
        }
        
        [HttpGet(ENDPOINT)]
        public async Task<List<EncoderDTO>> GetAvailableEncoders()
        {
            _logger.logEndpointRequestLogMessage(ENDPOINT, ENDPOINT_TYPE);

            List<EncoderDTO> availableEncodersFromFfmpeg = await _ffmpeg.getAvailableEncoders();

            return availableEncodersFromFfmpeg;
        }
    }
}