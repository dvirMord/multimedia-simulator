using video_simulator.constans;
using video_simulator.DTOs;
using video_simulator.Interfaces;

namespace video_simulator.Services
{
    public class VideoSimulatorService : IVideoSimulatorService
    {
        private readonly IFFmpegManager _ffmpegService;
        private readonly IStorageService _storageManagementService;

        public VideoSimulatorService(IFFmpegManager ffmpegService, IStorageService storageService)
        {
            this._ffmpegService = ffmpegService;
            this._storageManagementService = storageService;
        }

        //files
        public async Task<bool> ReceiveFileAsync(IFormFile file)
        {
            StoredFileResult newFile = await this._storageManagementService.SaveFileAsync(file);
            return true;
        }

        public bool DeleteFile(string fileName)
        {
            this._storageManagementService.DeleteFile(fileName); 
            return true;
        }

        //streams
        public void StartRtspStream(string fileName)
        {
            this._ffmpegService.StartStream(fileName);
        }
        public void StopRtspStream(string streamName)
        {
            this._ffmpegService.StopStream(streamName);
        }
    }

}