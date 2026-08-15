namespace multimedia_simulator.Interfaces
{
    public interface IRtspStreamsService
    {
        //streams
        Task StartRtspStreamAsync(string fileName);
        Task StopRtspStreamAsync(string streamName);
    }
}

