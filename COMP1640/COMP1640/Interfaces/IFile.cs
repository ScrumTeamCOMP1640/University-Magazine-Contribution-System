namespace COMP1640.Interfaces
{
    public interface IFile
    {
        Task<bool> UploadFile (IFormFile fileName,  string folderName);

        Task<(byte[], string, string)> DownloadFile (string folder, int? id);
        
        Task<(byte[], string, string)> DownloadAll ();
    }
}
