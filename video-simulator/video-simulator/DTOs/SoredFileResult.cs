namespace VideoSimulator.DTOs;

public record StoredFileResult
(
    string UniqueFileName,
    string OriginalFileName,
    string RelativePath,
    string FullPath,
    long FileSizeBytes,
    string Extension
);