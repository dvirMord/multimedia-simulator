namespace multimedia_simulator.constants
{
    public static class Constants
    {
        // ffmpeg files
        public const string TS_FILE_EXTENSION = ".ts";
        public const int TRANSMITTED_FILE_SIZE = 500 * 1024 * 1024;

        // mediaMTX server
        public const string RTSP_STREAM_URL = "rtsp://localhost:8554/";
        public const int RTSP_DEFAULT_PORT = 8554;
        public const string LOOPBACK_IP = "127.0.0.1";
        public const int WAIT_FOR_PORT_MILLISECONDS = 100;
        public const int MEDIAMTX_STARTUP_TIMEOUT_SECONDS = 5;
        public const int MEDIAMTX_SHUTDOWN_TIMEOUT_MILLISECONDS = 3000;

        // ffmpeg process
        public const int FFMPEG_STARTUP_CHECK_MILLISECONDS = 500;
        public const int WAIT_FOR_FFMPEG_STREAM_TO_CLOSE = 5;

        //sql
        public const int BadInsertResponseCode = 0;
        public static class SwaggerConstants
        {
            public const string RoutePrefix = "swagger";
            public const string EndpointUrlTemplate = "/openapi/{0}.json";
            public const string EndpointNameTemplate = "API {0}";
        }
    }
}