namespace multimedia_simulator.constants
{
    public static class FFmpegManagerMessages
    {
        public static class Success
        {
            public const string StreamRunningTemplate = "Stream '{0}' is running via RTSP!";
            public const string StreamStoppedTemplate = "Stopping stream '{0}'...";
        }

        public static class Error
        {
            public const string ProcessCrashedImmediately = "Process CRASHED immediately!";
            public const string FFmpegErrorOutputTemplate = "FFmpeg Error Output:\n{0}";
            public const string FailedToStartStreamTemplate = "Failed to start stream: {0}";
            public const string StreamNotFoundInDb = "Source file with SimId {0} was not found.";
        }
        public static class Warning
        {
            public const string FFmpegStreamCloseTimeoutTemplate = "Process for stream {0} took too long to exit.";
            public const string StreamAlreadyActiveTemplate = "Stream '{0}' is already active and streaming.";
            public const string StreamNotActiveTemplate = "Stream '{0}' is not currently active.";

        }
    }
}