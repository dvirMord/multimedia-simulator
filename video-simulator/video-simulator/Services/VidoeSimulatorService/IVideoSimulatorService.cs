namespace video_simulator.Interfaces
{
    public interface IVideoSimulatorService
    {
        //files
        Task<bool> ReceiveFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string fileName);
        //streams
        Task StartRtspStreamAsync(string fileName);
        Task StopRtspStreamAsync(string streamName);
    }
}
