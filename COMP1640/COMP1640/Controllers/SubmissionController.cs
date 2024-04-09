using COMP1640.Interfaces;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;

namespace COMP1640.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly ILogger<SubmissionController> logger;
        private readonly UmcsContext context;
        private readonly IFile _file;
        private readonly IEmail _email;

        public SubmissionController (ILogger<SubmissionController> logger, UmcsContext context, IFile file, IEmail email)
        {
            this.logger = logger;
            this.context = context;
            this._file = file;
            this._email = email;
        }

        public IActionResult Submission()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submission(Article article, IFormFile file, IFormFile img)
        {
            var student = context.Users.FirstOrDefault(u => u.Username == HttpContext.Session.GetString("Username"));
            if (student == null)
            {
                return NotFound("Cannot find student");
            }

            var coor = context.Users.Where(x => x.RoleId == 3 && x.FacultyId == student.FacultyId).FirstOrDefault();
            if (coor == null)
            {
                return NotFound("Cannot find coordinator");
            }

            var term = context.Terms.Where(x => x.TermName.Equals(HttpContext.Session.GetString("Term"))).FirstOrDefault();
            if (term == null)
            {
                return NotFound("Cannot find term");
            }

            if (ModelState.IsValid)
            {
                article.Content = await Upload(file, article.Title);
                article.ImagePath = await Upload(img, article.Title);
                article.UserId = student.UserId;
                article.SubmissionDate = DateTime.Now;
                article.TermId = term.TermId;
                article.FacultyId = student.FacultyId;
                article.Status = "Pending";
                context.Add(article);
                await context.SaveChangesAsync();

                await SendMail(coor);

                return RedirectToAction("Index", "Home");
            }
            return View(article);
        }

        public IActionResult ContributeDetail(int? id)
        {
            var article = context.Articles.FirstOrDefault(a => a.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            ViewBag.WordHtml = ReadFile(article);
            ViewBag.Users = context.Users.ToList();
            return View(article);
        }

        public async System.Threading.Tasks.Task SendMail(User user)
        {
            var email = "id22052003@gmail.com";
            var subject = "Contribute announce";
            var message = user.Username;
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

        public string ReadFile(Article article)
        {
            var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles", article.Title.Replace(" ", "_")));
            var files = Directory.GetFiles(path).Where(x => x.EndsWith(".doc"));
            foreach (var file in files)
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    object readOnly = true;
                    Application application = new Application();
                    Document document = new Document();

                    document = application.Documents.Open(file, readOnly);

                    string content = document.Content.Text;
                    document.Close();
                    application.Quit();
                    return content;
                }
            }
            return "null";
        }
    }
}
