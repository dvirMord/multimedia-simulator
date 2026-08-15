using Microsoft.AspNetCore.Mvc;
using multimedia_simulator.constans;
using multimedia_simulator.Interfaces;

namespace multimedia_simulator.Controllers
{
    [ApiController]
    [Route("api/ms")]
    public class RtspStreamsController: ControllerBase
    {
        private readonly IRtspStreamsService _rtspStreamsService;

        public RtspStreamsController(IRtspStreamsService rtspStreamsService)
        {
            this._rtspStreamsService = rtspStreamsService;
        }   

        //-------------------- APIs for streams-----------------
        [HttpPost("stream/start")]
        public async Task<IActionResult> StartStreamAsync([FromBody] DTOs.StartStreamDTO request)
        {
            try
            {
                await this._rtspStreamsService.StartRtspStreamAsync(request);
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
                await this._rtspStreamsService.StopRtspStreamAsync(request.StreamName);
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
