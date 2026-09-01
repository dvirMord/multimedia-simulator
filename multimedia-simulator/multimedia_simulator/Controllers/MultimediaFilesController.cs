using Microsoft.AspNetCore.Mvc;
using multimedia_simulator.constants;
using multimedia_simulator.Interfaces;
using Asp.Versioning;

namespace multimedia_simulator.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ms")]
    public class MultimediaFilesController : ControllerBase
    {
        private readonly IMultimediaFilesService _multimediaSimulatorService;
        private readonly ILogger<MultimediaFilesController> _logger;

        public MultimediaFilesController(
            IMultimediaFilesService multimediaSimulatorService,
            ILogger<MultimediaFilesController> logger)
        {
            this._multimediaSimulatorService = multimediaSimulatorService;
            this._logger = logger;
        }

        // -------------APIs for files---------------
        [HttpPost("files")]
        [RequestSizeLimit(Constants.TRANSMITTED_FILE_SIZE)]
        public async Task<IActionResult> ReceiveFileAsync(IFormFile file)
        {
            this._logger.LogInformation(FilesLoggerMessages.ReciveFilelog, file.FileName);
            try
            {
                int newIdInDb = await _multimediaSimulatorService.ReceiveFileAsync(file);
                return Ok(new
                {
                    success = true,
                    message = FilesControllerMessages.Success.FileReciveAndSave,
                    idInDb = newIdInDb
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
        public async Task<IActionResult> DeleteFileAsync([FromBody] DTOs.DeleteFileDTO request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = FilesControllerMessages.Error.RequestNull
                });
            }

            this._logger.LogInformation(FilesLoggerMessages.DeleteFile, request.SimId);
            try
            {
                bool result = await _multimediaSimulatorService.DeleteFileAsync(request.SimId);
                return Ok(new
                {
                    success = result,
                    message = string.Format(FilesControllerMessages.Success.DeleteSuccessTemplate, request.SimId)
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    success = false,
                    message = string.Format(FilesControllerMessages.Error.FileNotFoundTemplate, request.SimId)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = string.Format(FilesControllerMessages.Error.FileDeleteFailedTemplate, request.SimId, ex.Message)
                });
            }
        }
    }
}