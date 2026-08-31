using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using multimedia_simulator.constans;
using multimedia_simulator.DTOs;
using multimedia_simulator.Interfaces;

namespace multimedia_simulator.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ms")]
    public class RtspStreamsController: ControllerBase
    {
        private readonly IRtspStreamsService _rtspStreamsService;

        public RtspStreamsController(IRtspStreamsService rtspStreamsService)
        {
            this._rtspStreamsService = rtspStreamsService;
        }

        //-------------------- APIs for streams-----------------
        [HttpPost("stream/start")]
        public async Task<IActionResult> StartStreamAsync([FromBody] DTOs.StartStreamByIdDTO request)
        {
            DTOs.StartStreamDTO startStreamDTO = await this.ConvertStartStreamForId(request);
            try
            {
                string streamEndPoint = await this._rtspStreamsService.StartRtspStreamAsync(startStreamDTO);
                return Ok(new DTOs.StreamResponseDTO
                {
                    Success = true,
                    Message = string.Format(StreamsControllerMessages.Success.StartStreamTriggeredTemplate, startStreamDTO.FileName),
                    RtspStream = streamEndPoint
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new DTOs.StreamResponseDTO
                {
                    Success = false,
                    Message = string.Format(StreamsControllerMessages.Error.StartStreamFailedTemplate, startStreamDTO.FileName, ex.Message),
                    RtspStream = string.Empty
                });
            }
        }



        [HttpPost("stream/stop")]
        public async Task<IActionResult> StopRtspStreamAsync([FromBody] DTOs.StopStreamByIdDTO request)
        {
            DTOs.StopStreamDTO dtoForService = await this.ConvertStopStreamForId(request);
            try
            {
                await this._rtspStreamsService.StopRtspStreamAsync(dtoForService.StreamName);
                return Ok(new
                {
                    success = true,
                    message = string.Format(StreamsControllerMessages.Success.StopStreamTriggeredTemplate, dtoForService.StreamName)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = string.Format(StreamsControllerMessages.Error.StopStreamFailedTemplate, dtoForService.StreamName, ex.Message)
                });
            }
        }

        private async Task<DTOs.StopStreamDTO> ConvertStopStreamForId(DTOs.StopStreamByIdDTO requset)
        {
            return await this._rtspStreamsService.MakeStopStreamForId(requset);
        }

        private async Task<DTOs.StartStreamDTO> ConvertStartStreamForId(DTOs.StartStreamByIdDTO requset)
        {
            return await this._rtspStreamsService.MakeStartStreamForId(requset);
           
        }
    }
}
