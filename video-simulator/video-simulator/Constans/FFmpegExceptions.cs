namespace video_simulator.constans
{
    public static class FFmpegExceptions
    {
        public const string FFmpegPathNotConfigured = "FFmpeg path is not configured.";
        public const string TsFilesStoragePathNotConfigured = "TS files storage path is not configured.";
        public const string StreamAlreadyRunningTemplate = "{0} Stream is already running";
        public const string StreamNotRunningTemplate = "Stream '{0}' is not running.";
        public const string StreamStartFailedTemplate = "Failed to start stream '{0}': {1}";
    }
}