namespace video_simulator.constans
{
    public static class MediaMtxServerMessages
    {
        public static class Success
        {
            public const string ServerListeningTemplate = "Server is bound and listening on RTSP port {0}";
            public const string ShuttingDown = "Shutting down MediaMTX process...";
        }

        public static class Error
        {
            public const string ProcessTerminatedUnexpectedlyTemplate = "PROCESS TERMINATED UNEXPECTEDLY! (ExitCode: {0})";
            public const string ShutdownExceptionTemplate = "Exception during shutdown: {0}";
        }

        public static class Output
        {
            public const string OutputTemplate = "[mediamtx] {0}";
            public const string ErrorTemplate = "[mediamtx] {0}";
        }
    }
}