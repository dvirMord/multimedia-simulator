namespace video_simulator.constans
{
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
}