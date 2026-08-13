using video_simulator.constans;
using VideoSimulator.DTOs;
using VideoSimulator.Services;

namespace video_simulator.Services
{
    public class VideoSimulatorService : IVideoSimulatorService
    {
        private readonly IFFmpegManager _ffmpegService;
        private readonly IStorageService _localStorgeService;

        public VideoSimulatorService(IFFmpegManager ffmpegService, IStorageService storageService)
        {
            this._ffmpegService = ffmpegService;
            this._localStorgeService = storageService;
        }

        //files
        public async Task<bool> ReceiveFileAsync(IFormFile file)
        {
            StoredFileResult newFile = await this._localStorgeService.SaveFileAsync(file);
            return true;
        }

        public bool DeleteFile(string fileName)
        {
            this._localStorgeService.DeleteFile(fileName); 
            return true;
        }

        //streams
        public bool StartRtspStream(string fileName)
        {
            this._ffmpegService.StartStream(fileName);
            return true;
        }
        public bool StopRtspStream(string streamName)
        {
            this._ffmpegService.StopStream(streamName);
            return true;
        }
    }

}