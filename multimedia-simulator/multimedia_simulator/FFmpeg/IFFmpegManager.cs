namespace multimedia_simulator.Interfaces
{
    public interface IFFmpegManager
    {
        public Task StartStreamAsync(string streamId);
        public Task StopStreamAsync(string streamId);
        public bool IsStreamRunning(string streamId);
        public List<string> GetRunningStreams();
        public Task KillAllRunningStreams();
    }
}