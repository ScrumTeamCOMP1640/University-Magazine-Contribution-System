using COMP1640.Interfaces;
using Microsoft.AspNetCore.StaticFiles;

namespace COMP1640.Services
{
    public class FileService : IFile
    {
        public async Task<bool> UploadFile(IFormFile file)
        {
            var path = string.Empty;

            try
            {
                if (file != null && file.Length <= 5242880 /*&& (file.ContentType.Equals("application/msword") || file.ContentType.Equals("image/*"))*/)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot" ,"UploadedFiles"));

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                        
                    using (var fs = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(byte[], string, string)> DownloadFile(string fileName)
        {
            try
            {
                var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedFiles", fileName));
                var provider = new FileExtensionContentTypeProvider();

                if (!File.Exists(path))
                {
                    throw new Exception("File not found.");
                }

                if (!provider.TryGetContentType(path, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var readAllBytes = await File.ReadAllBytesAsync(path);
                return (readAllBytes, contentType, Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
