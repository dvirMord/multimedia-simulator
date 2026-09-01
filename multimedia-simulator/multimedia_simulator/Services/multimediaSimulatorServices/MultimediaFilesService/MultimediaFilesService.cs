using multimedia_simulator.DTOs;
using multimedia_simulator.Interfaces;
using multimedia_simulator.constants;

namespace multimedia_simulator.Services
{
    public class MultimediaFilesService : IMultimediaFilesService
    {
        private readonly IFFmpegManager _ffmpegService;
        private readonly IStorageService _storageManagementService;
        private readonly IDBManager _DBService;

        public MultimediaFilesService(IFFmpegManager ffmpegService,
            IStorageService storageService, IDBManager dbService)
        {
            this._ffmpegService = ffmpegService;
            this._storageManagementService = storageService;
            this._DBService = dbService;
        }

        public async Task<int> ReceiveFileAsync(IFormFile file)
        {
            StoredFileResult newFile = await this._storageManagementService.SaveFileAsync(file);
            int newId = await this._DBService.AddSourceFileAsync(file.FileName, file.Length);
            if(newId == Constants.BadInsertResponseCode)
            {
                throw new InvalidOperationException(string.Format(DBManagerExceptions.DBFalifToInsert, file.FileName));
            }
            return newId;
        }

        public async Task<bool> DeleteFileAsync(int simId)
        {
            string? filePath = await this._DBService.GetSourceFilePathByIdAsync(simId);
            if (string.IsNullOrEmpty(filePath))
            {
                throw new KeyNotFoundException(string.Format(FilesControllerMessages.Error.FileNotFoundTemplate, simId));
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await this._DBService.DeleteSourceFileAsync(Path.GetFileName(filePath));
            return true;
        }
    }
}
