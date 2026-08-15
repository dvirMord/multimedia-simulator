using Microsoft.Data.Sqlite;
using Dapper;
using multimedia_simulator.Interfaces;
using multimedia_simulator.constans;
using video_simulator.constans;

namespace multimedia_simulator.Services
{
    public class SQLiteManager: IDBManager, IDisposable
    {
        private string _storedFilesPath;
        private SqliteConnection _sqliteConnection = null!;
        private readonly SemaphoreSlim _semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1); // To ensure thread safety for database operations

        // --------------------constructor----------------
        public SQLiteManager(string path)
        {
            this._storedFilesPath = 
                Environment.GetEnvironmentVariable(EnvConstants.tsFilesStorageName) ??
            throw new InvalidOperationException(FFmpegExceptions.TsFilesStoragePathNotConfigured);
            this.CreateConnection(path);
        }

        //--------------------interface functions-------------------
        public void CreateConnection(string path)
        {
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder
            {
                DataSource = path,
                ForeignKeys = true
            };
            this._sqliteConnection = new SqliteConnection(builder.ConnectionString);
            this._sqliteConnection.Open();
        }

        public async Task InitializeDatabaseAsync()
        {
            string initScript = this.AllTables();
            await this.ExecuteAsync(initScript);
        }

        //--------------------Dapper wrapper functions(sqlite commands)-------------------
        public async Task<int> ExecuteAsync(string query, object? parameters = null)
        {
            await this._semaphoreLock.WaitAsync();
            try
            {
                return await this._sqliteConnection.ExecuteAsync(query, parameters);
            }
            finally
            {
                _semaphoreLock.Release();
            }
        }
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
            await _semaphoreLock.WaitAsync();
            try
            {
                return await this._sqliteConnection.QueryAsync<T>(query, parameters);
            }
            finally
            {
                _semaphoreLock.Release();
            }
        }
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string query, object? parameters = null)
        {
            await _semaphoreLock.WaitAsync();
            try
            {
                return await this._sqliteConnection.QuerySingleOrDefaultAsync<T>(query, parameters);
            }
            finally
            {
                _semaphoreLock.Release();
            }
        }
        //-----------------------------------------------------------------------------------

        //--------- functions for file management-------------------------------------------
        public async Task<int> AddSourceFileAsync(string fileName, long fileSize) 
        {
            string filePath = Path.Combine(this._storedFilesPath, fileName);
            
            int newId = await this.QuerySingleOrDefaultAsync<int>(
                SqlQueries.SourceFiles.Insert, 
                new { Path = filePath, Size = fileSize });
            return newId;
        }

        public async Task<bool> DeleteSourceFileAsync(string fileName)
        {
            string filePath = Path.Combine(this._storedFilesPath, fileName);
            int rowsAffected = await this.ExecuteAsync(
                SqlQueries.SourceFiles.DeleteByPath, 
                new { Path = filePath });
            return rowsAffected > 0;
        }
        //-----------------------------------------------------------------------------------

        //-----------------functions for channels/RTSP streams management---------------------------------------------
        public async Task<int> AddChannelAsync(int deviceId, int sourceFilesId, string streamEndpoint, string type, int pid)
        {
            int newId = await this.QuerySingleOrDefaultAsync<int>(
                SqlQueries.Channels.Insert,
                new
                {
                    DeviceId = deviceId,
                    SourceFilesId = sourceFilesId,
                    StreamEndpoint = streamEndpoint,
                    Type = type,
                    FFmpegProcessId = pid
                });

            return newId;
        }

        public async Task<bool> DeleteChannelAsync(string channelEndPoint)
        {
            int rowsAffected = await this.ExecuteAsync(
                SqlQueries.Channels.DeleteByEndpoint,
                new { StreamEndpoint = channelEndPoint });

            return rowsAffected > 0;
        }
        //---------------------------------------------------------------------------------------------------  

        // --------------------private/helper functions-------------------
        private string AllTables()
        {
            const string initScript = @"
        -- הגדרות SQLite לביצועים ותמיכה בקשרי גומלין
        PRAGMA journal_mode = WAL;
        PRAGMA foreign_keys = ON;

        -- 1. טבלת מכשירים (Devices)
        CREATE TABLE IF NOT EXISTS Devices (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Ip TEXT NOT NULL,
            Port INTEGER NOT NULL,
            CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
        );

        -- 2. טבלת קבצי מדיה (SourceFiles)
        CREATE TABLE IF NOT EXISTS SourceFiles (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Path TEXT NOT NULL UNIQUE,
            Size INTEGER NOT NULL
        );

        -- 3. טבלת ערוצים (Channels)
        CREATE TABLE IF NOT EXISTS Channels (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            DeviceId INTEGER NOT NULL,
            SourceFilesId INTEGER NOT NULL,
            StreamEndpoint TEXT NOT NULL,
            Type TEXT NOT NULL,
            FFmpegProcessId INTEGER NULL,
            CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
            
            CONSTRAINT FK_Channels_Devices FOREIGN KEY (DeviceId) 
                REFERENCES Devices(Id) ON DELETE CASCADE,
                
            CONSTRAINT FK_Channels_SourceFiles FOREIGN KEY (SourceFilesId) 
                REFERENCES SourceFiles(Id) ON DELETE CASCADE
        );

        -- אינדקסים לשיפור ביצועים
        CREATE INDEX IF NOT EXISTS IX_Channels_DeviceId ON Channels (DeviceId);
        CREATE INDEX IF NOT EXISTS IX_Channels_SourceFilesId ON Channels (SourceFilesId);
    ";

            return initScript;
        }

        //--------------------IDisposable cleanup-------------------
        public void Dispose()
        {
            this._sqliteConnection?.Dispose();
            this._semaphoreLock?.Dispose();
        }
    }
}
