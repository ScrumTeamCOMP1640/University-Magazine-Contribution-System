using COMP1640.Interfaces;
using COMP1640.Models;
using COMP1640.Services;
using Microsoft.AspNetCore.Mvc;

namespace COMP1640.Controllers
{
    public class FileController : Controller
    {
        private readonly IFile _file;

        public FileController(IFile file)
        {
            _file = file;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (await _file.UploadFile(file))
                {
                    ViewBag.Message = "File uploaded successfully";
                }
                else
                {
                    ViewBag.Message = "File uploaded unsuccessfully";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error" + ex.Message;
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var file = await _file.DownloadFile(fileName);
            return File(file.Item1, file.Item2, file.Item3);
        }
    }
}
