using System.Data;

namespace multimedia_simulator.Interfaces
{
    public interface IDBManager
    {
        void CreateConnection(string path);
        Task InitializeDatabaseAsync();

        //Used for write operations where you do not expect table rows back,
        //only a confirmation of how many rows changed.
        Task<int> ExecuteAsync(string query, object? parameters = null);

        //Used for read operations that can return zero, one, or many rows
        Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null);

        //Used for lookups where at most one record is expected
        Task<T?> QuerySingleOrDefaultAsync<T>(string query, object? parameters = null);

        //--------------------functions for files-------------------
        Task<int> AddSourceFileAsync(string fileName, long fileSize);
        Task<bool> DeleteSourceFileAsync(string fileName);
        //---------------------functions for streams--------------------
        Task<int> AddChannelAsync(int sourceFilesId, string streamEndpoint, string type, int pid);
        Task<bool> DeleteChannelAsync(string channelEndPoint);

    }
}
