namespace multimedia_simulator.Interfaces
{
    public interface IMultimediaFilesService
    {
        //files
        Task<bool> ReceiveFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string fileName);
    }
}

