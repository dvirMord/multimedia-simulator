using multimedia_simulator.Interfaces;
using multimedia_simulator.DTOs;
using Microsoft.VisualBasic;
using multimedia_simulator.constans;

namespace multimedia_simulator.Services
{
    public class RtspStreamsService: IRtspStreamsService
    {
        private readonly IFFmpegManager _ffmpegService;
        private readonly IDBManager _dbManager;

        public RtspStreamsService(IFFmpegManager ffmpegService, IDBManager dBManager)
        {
            this._dbManager = dBManager;
            this._ffmpegService = ffmpegService;
        }

        //streams
        public async Task StartRtspStreamAsync(StartStreamDTO request)
        {
            int pid = await this._ffmpegService.StartStreamAsync(request.FileName);

            //add channel to db
            string streamEndPoint = constans.Constants.RTSP_STREAM_URL + request.FileName;

            int newId = await this._dbManager.AddChannelAsync(
                request.DeviceId, request.SourceFileId, streamEndPoint, request.Type.ToString(), pid);
            if (newId == constans.Constants.BadInsertResponseCode)
            {
                throw new InvalidOperationException(
                    string.Format(DBManagerExceptions.DBFalidToInsertChannel, request.FileName));
            }
        }
        public async Task StopRtspStreamAsync(string streamName)
        {
            string streamEndPoint = constans.Constants.RTSP_STREAM_URL + streamName;
            await this._dbManager.DeleteChannelAsync(streamEndPoint);
            await this._ffmpegService.StopStreamAsync(streamName);

        }
    }
}
