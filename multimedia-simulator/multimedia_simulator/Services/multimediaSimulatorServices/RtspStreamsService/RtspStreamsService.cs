using multimedia_simulator.Interfaces;

namespace multimedia_simulator.Services
{
    public class RtspStreamsService: IRtspStreamsService
    {
        private readonly IFFmpegManager _ffmpegService;

        public RtspStreamsService(IFFmpegManager ffmpegService)
        {
            this._ffmpegService = ffmpegService;
        }

        //streams
        public async Task StartRtspStreamAsync(string fileName)
        {
            await this._ffmpegService.StartStreamAsync(fileName);
        }
        public async Task StopRtspStreamAsync(string streamName)
        {
            await this._ffmpegService.StopStreamAsync(streamName);
        }
    }
}
