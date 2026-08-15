using multimedia_simulator.DTOs;
using multimedia_simulator.Interfaces;
using multimedia_simulator.constans;

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

        public async Task<bool> ReceiveFileAsync(IFormFile file)
        {
            StoredFileResult newFile = await this._storageManagementService.SaveFileAsync(file);
            int newId = await this._DBService.AddSourceFileAsync(file.FileName, file.Length);
            if(newId == Constants.BadInsertResponseCode)
            {
                throw new InvalidOperationException(string.Format(DBManagerExceptions.DBFalifToInsert, file.FileName));
            }
            return true;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            //check if stream is running and stop it before deleting the file
            if (this._ffmpegService.IsStreamRunning(fileName))
            {
                await this._ffmpegService.StopStreamAsync(fileName);
            }

            this._storageManagementService.DeleteFile(fileName);
            bool isDeleted = await this._DBService.DeleteSourceFileAsync(fileName);
            if (!isDeleted)
            {
                throw new InvalidOperationException(
                    string.Format(DBManagerExceptions.DBFalidToDelete, fileName));
            }
            return true;
        }
    }
}
