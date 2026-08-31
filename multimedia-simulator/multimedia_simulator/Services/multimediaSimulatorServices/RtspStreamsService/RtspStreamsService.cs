using multimedia_simulator.Interfaces;
using multimedia_simulator.DTOs;
using multimedia_simulator.constans;
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
            string streamEndPoint = constans.Constants.RTSP_STREAM_URL + request.FileName;

            var existingChannel = await this._dbManager.GetChannelByEndpointAsync<ChannelDTO>(streamEndPoint);

            if (existingChannel != null)
            {
                if (existingChannel.IsActive)
                {
                    throw new InvalidOperationException($"Stream '{request.FileName}' is already active and streaming.");
                }

                int pid = await this._ffmpegService.StartStreamAsync(request.FileName);
                await this._dbManager.UpdateChannelActiveStatusAsync(streamEndPoint, isActive: true, processId: pid);
                return streamEndPoint;
            }

            int newPid = await this._ffmpegService.StartStreamAsync(request.FileName);

            int newId = await this._dbManager.AddChannelAsync(
                request.SourceFileId, streamEndPoint, request.Type.ToString(), newPid);

            if (newId == constans.Constants.BadInsertResponseCode)
            {
                await this._ffmpegService.StopStreamAsync(request.FileName);
                throw new InvalidOperationException(
                    string.Format(DBManagerExceptions.DBFalidToInsertChannel, request.FileName));
            }

            return streamEndPoint;
        }

        public async Task StopRtspStreamAsync(string streamName)
        {
            string streamEndPoint = constans.Constants.RTSP_STREAM_URL + streamName;

            var existingChannel = await this._dbManager.GetChannelByEndpointAsync<ChannelDTO>(streamEndPoint);

            if (existingChannel == null || !existingChannel.IsActive)
            {
                throw new InvalidOperationException($"Stream '{streamName}' is not currently active.");
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