using Microsoft.AspNetCore.Mvc;
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
        [RequestSizeLimit(Constants.TRANSMITTED_FILE_SIZE)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                bool result = await _videoSimulatorService.ReceiveFileAsync(file);
                return Ok(new 
                {
                    success = true, 
                    message = string.Format(FilesControllerMessages.Success.FileReciveAndSave)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = string.Format(FilesControllerMessages.Error.FileSaveFailedTemplate, ex.Message)
                });
            }
        }


        [HttpDelete("files/{fileName}")]
        public IActionResult DeleteFile(string fileName)
        {
            try
            {
                bool result = _videoSimulatorService.DeleteFile(fileName);
                return Ok(new 
                {
                    success = result, 
                    message = string.Format(FilesControllerMessages.Success.DeleteSuccessTemplate, fileName)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = string.Format(FilesControllerMessages.Error.FileDeleteFailedTemplate, fileName)   
                });
            }
        }


        [HttpPost("stream/start")]
        public IActionResult StartStream([FromBody] StartStreamRequest request)
        {
            try
            {
                bool result = _videoSimulatorService.StartRtspStream(request.FileName);
                return Ok(new 
                {
                    success = result,
                    message = string.Format(StreamsControllerMessages.Success.StartStreamTriggeredTemplate, request.FileName)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false,
                    message = string.Format(StreamsControllerMessages.Error.StartStreamFailedTemplate, request.FileName, ex.Message)
                });
            }
        }

            
        [HttpPost("stream/stop")]
        public IActionResult StopStream([FromBody] StopStreamRequest request)
        {
            try
            {
                bool result = _videoSimulatorService.StopRtspStream(request.StreamName);
                return Ok(new 
                {
                    success = result,
                    message = string.Format(StreamsControllerMessages.Success.StopStreamTriggeredTemplate, request.StreamName)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = string.Format(StreamsControllerMessages.Error.StopStreamFailedTemplate, request.StreamName, ex.Message)
                });
            }
        }
    }
}