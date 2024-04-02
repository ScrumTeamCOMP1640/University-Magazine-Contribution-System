using COMP1640.Interfaces;
using COMP1640.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO.Compression;

namespace COMP1640.Services
{
    public class FileService : IFile
    {
        UmcsContext _context = new UmcsContext();
        public async Task<bool> UploadFile(IFormFile file, string folderName)
        {
            try
            {
                var path = string.Empty;

                if (file != null && file.Length <= 5242880)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot" ,"UploadedFiles", folderName.Replace(" ", "_")));

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

        public async Task<(byte[], string, string)> DownloadFile(string folder, int? id)
        {
            try
            {
                var article = _context.Articles.FirstOrDefault(x => x.ArticleId == id);
                var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles", article.Title.Replace(" ", "_")));
                var files = Directory.GetFiles(path);
                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            var fileInfo = new FileInfo(file);
                            var entry = zipArchive.CreateEntry(article.Title.Replace(" ", "_") + "/" + fileInfo.Name);

                            using (var entryStream = entry.Open())
                            {
                                using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                                {
                                    await fileStream.CopyToAsync(entryStream);
                                }
                            }
                        }
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return (memoryStream.ToArray(), "application/zip", article.Title.Replace(" ", "_") + ".zip");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<(byte[], string, string)> DownloadAll()
        {
            try
            {
                var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles"));

                var folders = Directory.GetDirectories(path);

                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var folder in folders)
                        {
                            var files = Directory.GetFiles(folder);
                            var folderInfo = new DirectoryInfo(folder);
                            foreach (var file in files)
                            {
                                var fileInfo = new FileInfo(file);
                                var entry = zipArchive.CreateEntry(folderInfo.Name + "/" + fileInfo.Name);

                                using (var entryStream = entry.Open())
                                {
                                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                                    {
                                        await fileStream.CopyToAsync(entryStream);
                                    }
                                }
                            }
                        }
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return (memoryStream.ToArray(), "application/zip", "Download.zip");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
