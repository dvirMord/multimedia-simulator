namespace multimedia_simulator.constans
{
    public static class FilesControllerMessages
    {
        public static class Success
        {
            public const string FileReciveAndSave = "File received and saved successfully.";
            public const string DeleteSuccessTemplate = "File with SimId '{0}' was deleted successfully.";
        }

        public static class Error
        {
            public const string RequestNull = "Request body is null.";
            public const string FileNotFoundTemplate = "Source file with SimId '{0}' was not found.";
            public const string FileSaveFailedTemplate = "Failed to save file: {0}";
            public const string FileDeleteFailedTemplate = "Failed to delete file with SimId '{0}'. Error: {1}";
        }
    }
}