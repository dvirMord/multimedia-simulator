namespace video_simulator.Services
{
    public interface IFfmpegManager
    {
        public bool StartStream(string streamId);
        public bool StopStream(string streamId);
        public bool IsStreamRunning(string streamId);
        public List<string> GetRunningStreams();
        public void KillAllRunningStreams();
    }
}
