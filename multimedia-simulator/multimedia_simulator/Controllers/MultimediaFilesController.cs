using Microsoft.AspNetCore.Mvc;
using multimedia_simulator.constans;
using multimedia_simulator.Interfaces;

namespace multimedia_simulator.Controllers
{
    [ApiController]
    [Route("api/ms")]
    public class MultimediaFilesController : ControllerBase
    {
        private readonly IMultimediaFilesService _multimediaSimulatorService;
        private readonly ILogger<MultimediaFilesController> _logger;
       

        public MultimediaFilesController(IMultimediaFilesService multimediaSimulatorService,
            ILogger<MultimediaFilesController> logger)
        {
            this._multimediaSimulatorService = multimediaSimulatorService;
            this._logger = logger;
            
        }

        //-------------APIs for files---------------
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
        public async Task<IActionResult> DeleteFileAsync([FromBody] DTOs.DeleteFileDTO requset)
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
    }
}
