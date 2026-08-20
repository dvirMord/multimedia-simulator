namespace multimedia_simulator.DTOs;

public record StoredFileResult
(
    string UniqueFileName,
    string FullPath,
    long FileSizeBytes,
    string Extension
);