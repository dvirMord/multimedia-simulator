namespace video_simulator.Interfaces
{
    public interface IVideoSimulatorService
    {
        //files
        Task<bool> ReceiveFileAsync(IFormFile file);
        bool DeleteFile(string fileName);
        //streams
        void StartRtspStream(string fileName);
        void StopRtspStream(string streamName);
    }
}
