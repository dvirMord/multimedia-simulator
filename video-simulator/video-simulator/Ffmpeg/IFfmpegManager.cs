namespace video_simulator.Services
{
    public interface IFFmpegManager
    {
        public bool StartStream(string streamId);
        public bool StopStream(string streamId);
        public bool IsStreamRunning(string streamId);
        public List<string> GetRunningStreams();
        public void KillAllRunningStreams();
    }
}