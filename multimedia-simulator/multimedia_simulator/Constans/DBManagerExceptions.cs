namespace multimedia_simulator.constans
{
    public class DBManagerExceptions
    {
        public const string DBPathNotConfigured = "File path not found in Environment Variables";
        public const string DBFalifToInsert = "Failed to insert file '{0}' into database.";
        public const string DBFalidToDelete = "Failed to delete file '{0}' from database.";

        public const string DBFalidToInsertChannel = "Failed to insert cannel '{0}' into database";
        public const string DBFailedToDeleteChannel = "Failed to delete cannel '{0}' from database";
    }
}
