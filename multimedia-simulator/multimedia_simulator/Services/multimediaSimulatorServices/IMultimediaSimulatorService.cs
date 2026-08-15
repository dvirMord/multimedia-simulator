namespace multimedia_simulator.Interfaces
{
    public interface IMultimediaSimulatorService
    {
        //files
        Task<bool> ReceiveFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string fileName);
        //streams
        Task StartRtspStreamAsync(string fileName);
        Task StopRtspStreamAsync(string streamName);
    }
}
