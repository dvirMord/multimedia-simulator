using video_simulator.DTOs;

namespace video_simulator.Interfaces;
public interface IStorageService
{
    Task<StoredFileResult> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    bool DeleteFile(string filePath, CancellationToken cancellationToken = default);
}