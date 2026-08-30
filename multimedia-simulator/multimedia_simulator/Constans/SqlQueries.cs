namespace video_simulator.constans
{
    public  class SqlQueries
    {
        // -------------------- SourceFiles Queries --------------------
        public  class SourceFiles
        {
            public const string GET_SOURCE_FILE_PATH_BY_ID = @"
                SELECT Path 
                FROM SourceFiles 
                WHERE Id = @Id;";
            public const string Insert = @"
                INSERT INTO SourceFiles (Path, Size) 
                VALUES (@Path, @Size);
                SELECT last_insert_rowid();";

            public const string DeleteByPath = "DELETE FROM SourceFiles WHERE Path = @Path;";
            public const string DeleteById = "DELETE FROM SourceFiles WHERE Id = @Id;";
            public const string GetById = "SELECT * FROM SourceFiles WHERE Id = @Id;";
            public const string GetAll = "SELECT * FROM SourceFiles;";
        }

        // -------------------- Devices Queries --------------------
        public class Devices
        {
            public const string Insert = @"
                INSERT INTO Devices (Ip, Port) 
                VALUES (@Ip, @Port);
                SELECT last_insert_rowid();";

            public const string GetAll = "SELECT * FROM Devices;";
            public const string GetById = "SELECT * FROM Devices WHERE Id = @Id;";
            public const string DeleteById = "DELETE FROM Devices WHERE Id = @Id;";
        }

        // -------------------- Channels Queries --------------------
        public class Channels
        {
            public const string Insert = @"
                INSERT INTO Channels (SourceFilesId, StreamEndpoint, Type, FFmpegProcessId) 
                VALUES (@SourceFilesId, @StreamEndpoint, @Type, @FFmpegProcessId);
                SELECT last_insert_rowid();";
            public const string UpdateProcessId = "UPDATE Channels SET FFmpegProcessId = @ProcessId WHERE Id = @Id;";
            public const string DeleteById = "DELETE FROM Channels WHERE Id = @Id;";
            public const string DeleteByEndpoint = "DELETE FROM Channels WHERE StreamEndpoint = @StreamEndpoint;";
        }
    }
}