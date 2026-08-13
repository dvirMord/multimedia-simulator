namespace video_simulator.constans
{
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