using Microsoft.AspNetCore.Mvc;
using Superpower.Model;
using video_simulator.constans;
using video_simulator.DTOs;
using video_simulator.Services;

namespace video_simulator.Controllers
{
    [ApiController]
    [Route("api/vs")]
    public class VideoSimulatorController : ControllerBase
    {
        private readonly IVideoSimulatorService _videoSimulatorService;

        public VideoSimulatorController(IVideoSimulatorService videoSimulatorService)
        {
            _videoSimulatorService = videoSimulatorService;
        }

        [HttpPost("files")]
        [RequestSizeLimit(Constants.FIVE_HUNDRED_MB)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                bool result = await _videoSimulatorService.UploadFileAsync(file);
                return Ok(new { success = true, message = "File uploaded successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Failed to upload file: {ex.Message}" });
            }
        }


        [HttpDelete("files/{fileName}")]
        public IActionResult DeleteFile(string fileName)
        {
            try
            {
                bool result = _videoSimulatorService.DeleteFile(fileName);
                return Ok(new { success = result, message = $"Deleted \"{fileName}\" successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Failed to delete file: {ex.Message}" });
            }
        }


        [HttpPost("stream/start")]
        public IActionResult StartStream([FromBody] StartStreamRequest request)
        {
            try
            {
                bool result = _videoSimulatorService.StartRtspStream(request.FileName);
                return Ok(new { success = result, message = $"Start stream triggered for {request.FileName}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Cant stream {request.FileName}, {ex.Message}"});
            }
        }


        [HttpPost("stream/stop")]
        public IActionResult StopStream([FromBody] StopStreamRequest request)
        {
            try
            {
                bool result = _videoSimulatorService.StopRtspStream(request.StreamName);
                return Ok(new { success = result, message = $"Stop stream triggered for {request.StreamName}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Cant stop stream {request.StreamName}, {ex.Message}" });
            }
        }
    }
}