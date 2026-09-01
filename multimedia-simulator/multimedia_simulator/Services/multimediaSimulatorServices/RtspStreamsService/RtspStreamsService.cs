using multimedia_simulator.Interfaces;
using multimedia_simulator.DTOs;
using multimedia_simulator.constants;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace multimedia_simulator.Services
{
    public class RtspStreamsService : IRtspStreamsService
    {
        private readonly IFFmpegManager _ffmpegService;
        private readonly IDBManager _dbManager;

        public RtspStreamsService(IFFmpegManager ffmpegService, IDBManager dBManager)
        {
            this._dbManager = dBManager;
            this._ffmpegService = ffmpegService;
        }

        public async Task<string> StartRtspStreamAsync(StartStreamDTO request)
        {
            string streamEndPoint = constants.Constants.RTSP_STREAM_URL + request.FileName;

            var existingChannel = await this._dbManager.GetChannelByEndpointAsync<ChannelDTO>(streamEndPoint);

            // if the channel already exists, check if it's active. If it is, throw an exception. If not, start the stream and update the channel's status to active.
            if (existingChannel != null)
            {
                if (existingChannel.IsActive)
                {
                    throw new InvalidOperationException(string.Format(FFmpegManagerMessages.Warning.StreamAlreadyActiveTemplate, request.FileName));
                }
                int pid = await this._ffmpegService.StartStreamAsync(request.FileName);
                await this._dbManager.UpdateChannelActiveStatusAsync(streamEndPoint, isActive: true, processId: pid);
                return streamEndPoint;
            }

            int newPid = await this._ffmpegService.StartStreamAsync(request.FileName);

            int newId = await this._dbManager.AddChannelAsync(
                request.SourceFileId, streamEndPoint, request.Type.ToString(), newPid);

            if (newId == constants.Constants.BadInsertResponseCode)
            {
                await this._ffmpegService.StopStreamAsync(request.FileName);
                throw new InvalidOperationException(
                    string.Format(DBManagerExceptions.DBFalidToInsertChannel, request.FileName));
            }

            return streamEndPoint;
        }

        public async Task StopRtspStreamAsync(string streamName)
        {
            string streamEndPoint = constants.Constants.RTSP_STREAM_URL + streamName;

            var existingChannel = await this._dbManager.GetChannelByEndpointAsync<ChannelDTO>(streamEndPoint);

            if (existingChannel == null)
            {
                throw new InvalidOperationException(string.Format(FFmpegManagerMessages.Error.StreamNotFoundInDb, streamName));
            }
            if(!existingChannel.IsActive)        
            {
                throw new InvalidOperationException(string.Format(FFmpegManagerMessages.Warning.StreamNotActiveTemplate, streamName));
            }

            await this._ffmpegService.StopStreamAsync(streamName);

            await this._dbManager.UpdateChannelActiveStatusAsync(streamEndPoint, isActive: false, processId: null);
        }

        public async Task<IEnumerable<ChannelDTO>> GetActiveStreamsAsync()
        {
            return await this._dbManager.GetAllActiveChannelsAsync<ChannelDTO>();
        }

        public async Task<DTOs.StopStreamDTO> MakeStopStreamForId(DTOs.StopStreamByIdDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            string? filePath = await this._dbManager.GetSourceFilePathByIdAsync(request.SimId);

            if (string.IsNullOrEmpty(filePath))
            {
                throw new KeyNotFoundException(string.Format(FFmpegManagerMessages.Error.StreamNotFoundInDb, filePath));
            }
            return new DTOs.StopStreamDTO
            {
                StreamName = Path.GetFileName(filePath)
            };
        }

        public async Task<DTOs.StartStreamDTO> MakeStartStreamForId(DTOs.StartStreamByIdDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            string? filePath = await this._dbManager.GetSourceFilePathByIdAsync(request.SimId);

            if (string.IsNullOrEmpty(filePath))
            {
                throw new KeyNotFoundException(string.Format(FFmpegManagerMessages.Error.StreamNotFoundInDb, filePath));
            }
            return new DTOs.StartStreamDTO
            {
                FileName = Path.GetFileName(filePath),
                SourceFileId = request.SimId
            };
        }
    }
}