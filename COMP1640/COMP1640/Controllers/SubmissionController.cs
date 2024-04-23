using COMP1640.Interfaces;
using COMP1640.Models;
using GroupDocs.Viewer;
using GroupDocs.Viewer.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace COMP1640.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IToastNotification _toast;
        private readonly IFile _file;
        private readonly IEmail _email;

        public SubmissionController (UmcsContext db, IToastNotification toast, IFile file, IEmail email)
        {
            _db = db;
            _toast = toast;
            _file = file;
            _email = email;
        }

        public IActionResult AddSubmission()
        {
            var student = _db.Users.FirstOrDefault(u => u.Email == HttpContext.Session.GetString("Email"));
            if (student == null)
            {
                return NotFound();
            }

            var faculty = _db.Faculties.FirstOrDefault(f => f.FacultyId == student.FacultyId);
            if (faculty == null)
            {
                return NotFound();
            }
            ViewBag.Faculty = faculty.FacultyName;

            var term = _db.Terms.FirstOrDefault(t => t.Status != null && t.Status.Equals("Active"));
            ViewBag.Term = term;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubmission(Article article, IFormFile file, IFormFile img)
        {
			var userEmail = HttpContext.Session.GetString("Email");

			var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			//var coor = _db.Users.FirstOrDefault(x => x.RoleId == 3 && x.FacultyId == user.FacultyId);
			//if (coor == null)
			//{
			//	return NotFound("Coordinator not found.");
			//}

			var term = await _db.Terms.FirstOrDefaultAsync(t => t.Status == "Active");
			if (term == null)
			{
				return NotFound("Active term not found.");
			}

			if (ModelState.IsValid)
            {
				article.Content = await Upload(file, article.Title);
				article.ImagePath = await Upload(img, article.Title);
				article.UserId = user.UserId;
				article.SubmissionDate = DateTime.Now;
				article.TermId = term.TermId;
				article.FacultyId = user.FacultyId;
                article.Status = "Pending";

                _db.Articles.Add(article);
                await _db.SaveChangesAsync();

				//await SendMail(coor);
				_toast.AddSuccessToastMessage("Article submitted successfully!");
				return RedirectToAction("Index", "Home");
			}

			_toast.AddErrorToastMessage("Failed to submit article!");
			return View(article);
		}

		public IActionResult EditSubmission(int? id)
		{
			if (id == null)
			{
				return NotFound("Article ID not provided.");
			}

			var article = _db.Articles.Find(id);
			if (article == null)
			{
				return NotFound("Article not found.");
			}

			var userEmail = HttpContext.Session.GetString("Email");

			var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			var faculty = _db.Faculties.FirstOrDefault(f => f.FacultyId == user.FacultyId);
			if (faculty == null)
			{
				return NotFound("Faculty not found.");
			}

			var term = _db.Terms.FirstOrDefault(t => t.Status != null && t.Status.Equals("Active"));
			if (term == null)
			{
				return NotFound("Active term not found.");
			}

			var fileName = article.Content;
			if (fileName != null)
			{
				string[] parts = fileName.Split('.');
				string extension = parts[1];
				ViewBag.FileName = fileName;
				ViewBag.Extension = extension;
			}

			ViewBag.Faculty = faculty.FacultyName;
			ViewBag.Term = term;

			return View(article);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditSubmission(int id, Article article, IFormFile file, IFormFile img)
		{
			var userEmail = HttpContext.Session.GetString("Email");

			var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			var term = await _db.Terms.FirstOrDefaultAsync(t => t.Status == "Active");
			if (term == null)
			{
				return NotFound("Active term not found.");
			}

			if (ModelState.IsValid)
			{
				var existingArticle = await _db.Articles.FindAsync(id);
				if (existingArticle == null)
				{
					return NotFound("Article not found.");
				}

				existingArticle.Title = article.Title;
				existingArticle.Content = await Upload(file, article.Title);
				existingArticle.ImagePath = await Upload(img, article.Title);
				existingArticle.SubmissionDate = DateTime.Now;
				existingArticle.TermId = term.TermId;
				existingArticle.FacultyId = user.FacultyId;
				existingArticle.Status = "Pending";

				_db.Entry(existingArticle).State = EntityState.Modified;
				await _db.SaveChangesAsync();

				_toast.AddSuccessToastMessage("Article updated successfully!");
				return RedirectToAction("Index", "Home");
			}

			_toast.AddErrorToastMessage("Failed to update article!");
			return View(article);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Comment(int id, string comment, string status)
		{
			var article = _db.Articles.FirstOrDefault(x => x.ArticleId == id);
			if (article == null)
			{
				return NotFound();
			}

			var userEmail = HttpContext.Session.GetString("Email");
			if (string.IsNullOrEmpty(userEmail))
			{
				return NotFound();
			}

			var user = _db.Users.FirstOrDefault(x => x.Email == userEmail);
			if (user == null)
			{
				return NotFound();
			}

			var coordinator = _db.Users.FirstOrDefault(x => x.FacultyId == user.FacultyId && x.RoleId == 3);
			if (coordinator == null)
			{
				return NotFound();
			}

			if (string.IsNullOrEmpty(comment) || !ModelState.IsValid)
			{
				_toast.AddErrorToastMessage("Failed to review article!");
				return RedirectToAction("ContributeDetail", new { id = article.ArticleId });
			}

			article.Status = status;

			var existingComment = _db.Comments.FirstOrDefault(c => c.ArticleId == article.ArticleId);

			if (existingComment != null)
			{
				existingComment.CommentContent = comment;
				existingComment.CommentDate = DateTime.Now;
				existingComment.UserId = coordinator.UserId;

				_db.Entry(existingComment).State = EntityState.Modified;
			}
			else
			{
				var newComment = new Comment
				{
					CommentContent = comment,
					CommentDate = DateTime.Now,
					UserId = coordinator.UserId,
					ArticleId = article.ArticleId
				};

				_db.Comments.Add(newComment);
			}

			_db.SaveChanges();

			_toast.AddSuccessToastMessage("Article reviewed successfully!");
			return RedirectToAction("ContributeDetail", new { id = article.ArticleId });
		}

        public async Task SendMail(User user)
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

        public Article Article (int? id)
        {
            var article = _db.Articles.Where(x => x.ArticleId == id).FirstOrDefault();
            if (article == null)
            {
                throw new Exception();
            }
            return article;
        }

        public string ContentPath ()
        {
            var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles"));
            return path;
        }

        public string OutPath()
        {
            var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "OutPut"));
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public IActionResult ViewDocument()
        {
            var id = HttpContext.Session.GetInt32("id");
            Article article = Article(id);
            if (article.Content == null)
                throw new Exception();

            string filePath = Path.Combine(ContentPath(), article.Title.Replace(" ", "_"), article.Content);
            string outFilePath = Path.Combine(OutPath(), article.Content.Split(".")[0] + ".pdf");

            using (Viewer viewer = new Viewer(filePath))
            {
                PdfViewOptions options = new PdfViewOptions(outFilePath);
                viewer.View(options);
            }

            FileStream fs = new FileStream(outFilePath, FileMode.Open, FileAccess.Read);
            FileStreamResult fsr = new FileStreamResult(fs, "application/pdf");
            return fsr  ;
        }
    }
}
