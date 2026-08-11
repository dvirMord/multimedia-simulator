namespace video_simulator.Services
{
    public interface IVideoSimulatorService
    {
        //files
        Task<bool> UploadFileAsync(IFormFile file);
        bool DeleteFile(string fileName);
        //streams
        bool StartRtspStream(string fileName);
        bool StopRtspStream(string streamName);
    }
}
