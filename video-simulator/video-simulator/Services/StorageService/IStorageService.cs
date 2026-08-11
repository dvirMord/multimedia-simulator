namespace VideoSimulator.Services;

using Microsoft.AspNetCore.Http;
using VideoSimulator.DTOs;

public interface IStorageService
{
    Task<StoredFileResult> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    bool DeleteFile(string filePath, CancellationToken cancellationToken = default);
}