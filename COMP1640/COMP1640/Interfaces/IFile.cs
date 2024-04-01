namespace COMP1640.Interfaces
{
    public interface IFile
    {
        Task<bool> UploadFile (IFormFile file);

        Task<(byte[], string, string)> DownloadFile (string fileName);
    }
}
