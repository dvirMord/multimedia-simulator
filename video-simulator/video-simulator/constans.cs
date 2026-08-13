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
        public const int TRANSMITTED_FILE_SIZE = 500 * 1024 * 1024;
        public const string RTSP_STREAM_URL = "rtsp://localhost:8554/";
    }
    public static class FilesControllerMessages
    {
        public static class Success
        {
            public const string FileReciveAndSave = "File recived and saved successfully.";
            public const string DeleteSuccessTemplate = "File: \"{0}\" Deleted successfully.";
        }
        public static class Error
        {
            public const string FileSaveFailedTemplate = "Failed to save file: {0}";
            public const string FileReciveFailedTemplate = "Failed to recive file: {0}";
            public const string FileDeleteFailedTemplate = "Failed to delete file: {0} {1}";
        }
    }

    public static class StreamsControllerMessages
    {
        public static class Success
        {
            public const string StartStreamTriggeredTemplate = "Start stream triggered for {0}";
            public const string StopStreamTriggeredTemplate = "Stop stream triggered for {0}";
        }
        public static class Error
        {
            public const string StartStreamFailedTemplate = "Cant stream {0}, {1}";
            public const string StopStreamFailedTemplate = "Cant stop stream {0}, {1}";
        }
    }
}
