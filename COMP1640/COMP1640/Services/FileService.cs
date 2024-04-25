using COMP1640.Interfaces;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO.Compression;

namespace COMP1640.Services
{
    public class FileService : IFile
    {
        private readonly UmcsContext _context;

        public FileService(UmcsContext context)
        {
            _context = context;
        }

		public async Task<bool> UploadFile(IFormFile file, string folderName)
		{
			try
			{
				var path = string.Empty;

				if (file != null && file.Length <= 5242880)
				{
					path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles", folderName));

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

		[HttpGet]
        public async Task<(byte[], string, string)> DownloadFile(string folder, int? id)
        {
            try
            {
                var article = _context.Articles.FirstOrDefault(x => x.ArticleId == id);
                if (article == null)
                {
                    throw new ArgumentException("Article not found.");
                }

                var content = article.Content;
                if (content == null)
                {
                    throw new ArgumentException("Article not found.");
				}

				var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles", article.ArticleId.ToString(), content));

                var provider = new FileExtensionContentTypeProvider();
                if (provider.TryGetContentType(path, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var readAllByte = await File.ReadAllBytesAsync(path);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return (readAllByte, contentType, Path.GetFileName(path));
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file: " + ex.Message);
            }
        }

        public async Task<(byte[], string, string)> DownloadAll()
        {
            try
            {
                var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles"));

                var folders = Directory.GetDirectories(path);

                var article = _context.Articles.Where(x => x.Status != null && x.Status.Equals("Approved")).ToList();
                if (article.Count() == 0)
                {
                    throw new Exception("No file found");
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var folder in folders)
                        {
                            var files = Directory.GetFiles(folder);
                            var folderInfo = new DirectoryInfo(folder);
                            foreach (var pub in article)
                            {
                                if (folderInfo.Name.Equals(pub.ArticleId.ToString()))
                                {
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
