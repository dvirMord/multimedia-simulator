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

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            //check if stream is running and stop it before deleting the file
            if (this._ffmpegService.IsStreamRunning(fileName))
            {
                await this._ffmpegService.StopStreamAsync(fileName);
            }
            
            this._storageManagementService.DeleteFile(fileName); 
            return true;
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