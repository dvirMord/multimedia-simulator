using multimedia_simulator.constants;
using multimedia_simulator.DTOs;
using multimedia_simulator.Validators;
using multimedia_simulator.Interfaces;

namespace multimedia_simulator.Services
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

            if(!File.Exists(fullPath))
            {
                throw new InvalidOperationException(string.Format(LocalStorageServiceExceptions.FileSaveFailedTemplate, file.FileName));
            }

            return new StoredFileResult
            (
                UniqueFileName: file.FileName,
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
            string filePath = Path.Combine(this._storagePath, fileName);
            File.Delete(filePath);
            if (!File.Exists(filePath))
            {
                return true;
            }
            throw new InvalidOperationException(string.Format(LocalStorageServiceExceptions.FileDeleteFailedTemplate, fileName));
        }
    }
}
