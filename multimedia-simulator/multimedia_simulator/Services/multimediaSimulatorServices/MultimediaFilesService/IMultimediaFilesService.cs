namespace multimedia_simulator.Interfaces
{
    public interface IMultimediaFilesService
    {
        //files
        Task<int> ReceiveFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string fileName);
    }
}

