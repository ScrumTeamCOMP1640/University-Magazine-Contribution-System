using System.Diagnostics;
using COMP1640.Interfaces;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UmcsContext _umcs;
        private readonly IEmail _email;
        private readonly IFile _file;

        public HomeController(ILogger<HomeController> logger, UmcsContext umcs, IEmail email, IFile file)
        {
            _logger = logger;
            _umcs = umcs;
            _email = email;
            _file = file;
        }

        public IActionResult Index(int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstSubmission = _umcs.Articles.AsNoTracking().OrderBy(_umcs => _umcs.ArticleId);
            PagedList<Article> model = new PagedList<Article>(lstSubmission, pageNumber, pageSize);
            ViewBag.Users = _umcs.Users.ToList();
            return View(model);
        }

        public IActionResult Submission()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submission(Article article, IFormFile file, IFormFile img)
        {
            var session = HttpContext.Session.GetString("Username");
            var student = _umcs.Users.FirstOrDefault(u => u.Username == session);

            if (student == null)
            {
                return NotFound();
            }

            var FaUs = _umcs.FaUs.ToList();
            var Coordinator = _umcs.Users.Where(u => u.RoleId == 3).ToList();
            var coordinator = new User();

            foreach (var faus in FaUs)
            {
                if (faus.UserId == student.UserId)
                {
                    var tempFaUs = FaUs.Where(f => f.FacultyId == faus.FacultyId).ToList();
                    foreach (var tempfaus in tempFaUs)
                    {
                        foreach (var co in Coordinator)
                        {
                            if (tempfaus.UserId == co.UserId)
                            {
                                coordinator = _umcs.Users.FirstOrDefault(u => u.UserId == co.UserId);
                                break;
                            }
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                article.Content = await Upload(file, article.Title);
                article.ImagePath = await Upload(img, article.Title);
                article.UserId = student.UserId;
                article.SubmissionDate = DateTime.Now;
                article.Status = "Pending";
                _umcs.Add(article);
                await _umcs.SaveChangesAsync();

                if (coordinator != null)
                {
                    await SendMail(coordinator);
                }

                return RedirectToAction("Index");
            }
            return View(article);
        }

        public IActionResult ContributeDetail(int? id)
        {
            var article = _umcs.Articles.FirstOrDefault(a => a.ArticleId == id);
            ViewBag.Users = _umcs.Users.ToList();
            return View(article);
        }

        public async Task SendMail(User user)
        {
            var email = "id22052003@gmail.com";
            var subject = "Contribute announce";
            var message = user.Email;
            await _email.SendEmailAsync(email, subject, message);
        }

        public async Task<string> Upload(IFormFile file, string folder)
        {
            if (await _file.UploadFile(file, folder))
                return file.FileName;
            throw new Exception("Error");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string folder, int? id)
        {
            var file = await _file.DownloadFile(folder, id);
            return File(file.Item1, file.Item2, file.Item3);
        }
        
        [HttpGet]
        public async Task<IActionResult> DownloadAll()
        {
            var file = await _file.DownloadAll();
            return File(file.Item1, file.Item2, file.Item3);
        }
    }
}
