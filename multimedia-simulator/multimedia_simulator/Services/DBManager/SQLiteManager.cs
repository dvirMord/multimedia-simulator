using Microsoft.Data.Sqlite;
using Dapper;
using multimedia_simulator.Interfaces;
using multimedia_simulator.constants;
using video_simulator.constants;

namespace multimedia_simulator.Services
{
    public class SQLiteManager : IDBManager, IDisposable
    {
        private string _storedFilesPath;
        private SqliteConnection _sqliteConnection = null!;
        private readonly SemaphoreSlim _semaphoreLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public SQLiteManager(string path)
        {
            this._storedFilesPath =
                Environment.GetEnvironmentVariable(EnvConstants.tsFilesStorageName) ??
                throw new InvalidOperationException(FFmpegExceptions.TsFilesStoragePathNotConfigured);
            this.CreateConnection(path);
        }

        public void CreateConnection(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder
            {
                DataSource = path,
                Mode = SqliteOpenMode.ReadWriteCreate,
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
            await this._semaphoreLock.WaitAsync();
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
            await this._semaphoreLock.WaitAsync();
            try
            {
                return await this._sqliteConnection.QuerySingleOrDefaultAsync<T>(query, parameters);
            }
            finally
            {
                _semaphoreLock.Release();
            }
        }

        // ----------------- functions for file management -----------------
        public async Task<int> AddSourceFileAsync(string fileName, long fileSize)
        {
            string filePath = Path.Combine(this._storedFilesPath, fileName);

            int newId = await this.QuerySingleOrDefaultAsync<int>(
                SqlQueries.SourceFiles.Insert,
                new { Path = filePath, Size = fileSize });
            return newId;
        }

        public async Task<string?> GetSourceFilePathByIdAsync(int id)
        {
            return await this.QuerySingleOrDefaultAsync<string>(
                SqlQueries.SourceFiles.GET_SOURCE_FILE_PATH_BY_ID,
                new { Id = id });
        }

        public async Task<bool> DeleteSourceFileAsync(string fileName)
        {
            string filePath = Path.Combine(this._storedFilesPath, fileName);
            int rowsAffected = await this.ExecuteAsync(
                SqlQueries.SourceFiles.DeleteByPath,
                new { Path = filePath });
            return rowsAffected > 0;
        }

        // ----------------- functions for channels/RTSP streams management -----------------
        public async Task<int> AddChannelAsync(int sourceFilesId, string streamEndpoint, string type, int? pid)
        {
            int newId = await this.QuerySingleOrDefaultAsync<int>(
                SqlQueries.Channels.Insert,
                new
                {
                    SourceFilesId = sourceFilesId,
                    StreamEndpoint = streamEndpoint,
                    Type = type,
                    FFmpegProcessId = pid
                });

            return newId;
        }

        public async Task<T?> GetChannelByEndpointAsync<T>(string endpoint)
        {
            return await this.QuerySingleOrDefaultAsync<T>(
                SqlQueries.Channels.GetByEndpoint,
                new { StreamEndpoint = endpoint });
        }

        public async Task<T?> GetChannelBySourceFileAndTypeAsync<T>(int sourceFilesId, string type)
        {
            return await this.QuerySingleOrDefaultAsync<T>(
                SqlQueries.Channels.GetBySourceFileAndType,
                new { SourceFilesId = sourceFilesId, Type = type });
        }

        public async Task<bool> UpdateChannelActiveStatusAsync(string streamEndpoint, bool isActive, int? processId = null)
        {
            int rowsAffected = await this.ExecuteAsync(
                SqlQueries.Channels.UpdateActiveStatus,
                new
                {
                    StreamEndpoint = streamEndpoint,
                    IsActive = isActive ? 1 : 0,
                    FFmpegProcessId = processId
                });

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<T>> GetAllActiveChannelsAsync<T>()
        {
            return await this.QueryAsync<T>(SqlQueries.Channels.GetAllActive);
        }

        public async Task<bool> DeleteChannelAsync(string channelEndPoint)
        {
            int rowsAffected = await this.ExecuteAsync(
                SqlQueries.Channels.DeleteByEndpoint,
                new { StreamEndpoint = channelEndPoint });

            return rowsAffected > 0;
        }

        // ----------------- private/helper functions -----------------
        private string AllTables()
        {
            const string initScript = @"
                PRAGMA journal_mode = WAL;
                PRAGMA foreign_keys = ON;

                CREATE TABLE IF NOT EXISTS SourceFiles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Path TEXT NOT NULL UNIQUE,
                    Size INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Channels (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SourceFilesId INTEGER NOT NULL,
                    StreamEndpoint TEXT NOT NULL,
                    Type TEXT NOT NULL,
                    FFmpegProcessId INTEGER NULL,
                    IsActive INTEGER NOT NULL DEFAULT 1,
                    CreatedAt TEXT NOT NULL DEFAULT (datetime('now', 'localtime')),
                        
                    CONSTRAINT FK_Channels_SourceFiles FOREIGN KEY (SourceFilesId) 
                        REFERENCES SourceFiles(Id) ON DELETE CASCADE,
                    CONSTRAINT UQ_Channels_Endpoint UNIQUE (StreamEndpoint)
                );

                CREATE INDEX IF NOT EXISTS IX_Channels_SourceFilesId ON Channels (SourceFilesId);
                CREATE INDEX IF NOT EXISTS IX_Channels_Active ON Channels (IsActive);
            ";

            return initScript;
        }

        public void Dispose()
        {
            this._sqliteConnection?.Dispose();
            this._semaphoreLock?.Dispose();
        }
    }
}