using VideoSimulator.Services;
using video_simulator.constans;
using VideoSimulator.DTOs;
using video_simulator.Validators;

namespace video_simulator.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _storagePath;

        public LocalStorageService()
        {
            this._storagePath =
                Environment.GetEnvironmentVariable(EnvConstants.tsFilesStorageName)
                ?? Environment.GetEnvironmentVariable(EnvConstants.defaultTsFilesStorageName)!;
            if (!Directory.Exists(this._storagePath))
            {
                Directory.CreateDirectory(this._storagePath);
            }
        }

        public async Task<StoredFileResult> SaveFileAsync(IFormFile file,
            CancellationToken cancellationToken = default)
        {
            MyValidators.ValidateNotNullOrEmpty(file);
            MyValidators.ValidateFileExtension(file.FileName);
            MyValidators.ValidateFileDoesNotExist(file.FileName, this._storagePath);
           
            string fullPath = Path.Combine(this._storagePath, file.FileName);
            using (FileStream stream =
                new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return new StoredFileResult
            (
                UniqueFileName: file.FileName,
                OriginalFileName: file.FileName,
                RelativePath: fullPath,
                FullPath: fullPath,
                FileSizeBytes: file.Length,
                Extension: Constants.TS_FILE_EXTENSION
            );
        }

        public bool DeleteFile(string fileName, CancellationToken cancellationToken = default)
        {
            MyValidators.ValidateNotNullOrEmpty(fileName);
            MyValidators.ValidateFileExtension(fileName);
            MyValidators.ValidateFileExists(fileName, this._storagePath);
        
            File.Delete(Path.Combine(this._storagePath, fileName));
            return true;
        }

        
    }
}
