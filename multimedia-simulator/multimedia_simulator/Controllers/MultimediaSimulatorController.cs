using Microsoft.AspNetCore.Mvc;
using multimedia_simulator.constans;
using multimedia_simulator.Interfaces;

namespace multimedia_simulator.Controllers
{
    //-------------APIs for files---------------
    [ApiController]
    [Route("api/vs")]
    public class MultimediaSimulatorController : ControllerBase
    {
        private readonly IMultimediaSimulatorService _multimediaSimulatorService;
        private readonly ILogger<MultimediaSimulatorController> _logger;

        public MultimediaSimulatorController(IMultimediaSimulatorService multimediaSimulatorService,
            ILogger<MultimediaSimulatorController> logger)
        {
            this._multimediaSimulatorService = multimediaSimulatorService;
            this._logger = logger;
        }

        [HttpPost("files")]
        [RequestSizeLimit(Constants.TRANSMITTED_FILE_SIZE)]
        public async Task<IActionResult> ReceiveFileAsync(IFormFile file)
        {
            this._logger.LogInformation(FilesLoggerMessages.ReciveFilelog, file.FileName);
            try
            {
                bool result = await _multimediaSimulatorService.ReceiveFileAsync(file);
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


        [HttpDelete("files")]
        public async Task<IActionResult> DeleteFileAsync([FromBody]  DTOs.DeleteFileDTO requset)
        {
            this._logger.LogInformation(FilesLoggerMessages.DeleteFile, requset.FileName);
            try
            {
                bool result = await _multimediaSimulatorService.DeleteFileAsync(requset.FileName);
                return Ok(new 
                {
                    success = result, 
                    message = string.Format(FilesControllerMessages.Success.DeleteSuccessTemplate, requset.FileName)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    success = false, 
                    message = string.Format(FilesControllerMessages.Error.FileDeleteFailedTemplate, requset.FileName, ex.Message)   
                });
            }
        }

        //-------------------- APIs for streams-----------------
        [HttpPost("stream/start")]
        public async Task<IActionResult> StartStreamAsync([FromBody] DTOs.StartStreamDTO request)
        {
            try
            {
                await _multimediaSimulatorService.StartRtspStreamAsync(request.FileName);
                return Ok(new 
                {
                    success = true,
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
        public async Task<IActionResult> StopRtspStreamAsync([FromBody] DTOs.StopStreamDTO request)
        {
            try
            {
                await _multimediaSimulatorService.StopRtspStreamAsync(request.StreamName);
                return Ok(new 
                {
                    success = true,
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