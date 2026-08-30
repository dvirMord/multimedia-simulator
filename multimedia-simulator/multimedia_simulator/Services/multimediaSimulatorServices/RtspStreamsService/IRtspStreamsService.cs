namespace multimedia_simulator.Interfaces
{
    public interface IRtspStreamsService
    {
        //streams
        Task<DTOs.StartStreamDTO> MakeStartStreamForId(DTOs.StartStreamByIdDTO request);

        Task<DTOs.StopStreamDTO> MakeStopStreamForId(DTOs.StopStreamByIdDTO request);
        Task StartRtspStreamAsync(DTOs.StartStreamDTO request);
        Task StopRtspStreamAsync(string streamName);
    }
}

