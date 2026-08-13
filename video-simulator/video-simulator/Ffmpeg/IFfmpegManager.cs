namespace video_simulator.Interfaces
{
    public interface IFFmpegManager
    {
        public void StartStream(string streamId);
        public void StopStream(string streamId);
        public bool IsStreamRunning(string streamId);
        public List<string> GetRunningStreams();
        public void KillAllRunningStreams();
    }
}