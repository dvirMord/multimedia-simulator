namespace video_simulator.constans
{
   public static class EnvConstants
    {
        public const string tsFilesStorageName = "MEDIA_STORAGE_PATH";
        public const string defaultTsFilesStorageName = "MEDIA_STORAGE_PATH_DEFAULT";
        public const string ffmpegPathName = "FFMPEG_PATH";
        public const string mediaMTXPathName = "MEDIAMTX_PATH";
    }

    public static class Constants
    {
        public const string TS_FILE_EXTENSION = ".ts";
        public const int FIVE_HUNDRED_MB = 500 * 1024 * 1024;
        public const string RTSP_STREAM_URL = "rtsp://localhost:8554/";
    }
}
