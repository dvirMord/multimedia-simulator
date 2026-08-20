namespace multimedia_simulator.Interfaces
{
    public interface IRtspStreamsService
    {
        //streams
        Task StartRtspStreamAsync(DTOs.StartStreamDTO request);
        Task StopRtspStreamAsync(string streamName);
    }
}

