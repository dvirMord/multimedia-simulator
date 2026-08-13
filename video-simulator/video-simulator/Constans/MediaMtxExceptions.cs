namespace video_simulator.constans
{
    public static class MediaMtxExceptions
    {
        public const string ExePathNullException = "MediaMTX executable path cannot be null.";
        public const string LoggerNullException = "Logger cannot be null.";
        public const string InvalidDirectoryPathTemplate = "Invalid directory path: {0}";
        public const string MediaMTXPathNotConfigured = "MediaMTX path is not configured.";
        public const string MediaMTXBinaryNotFound = "MediaMTX binary not found at specified path.";
        public const string RtspListenerFailedTemplate = "MediaMTX RTSP listener failed to start on port {0} within {1}s.";
    }
}