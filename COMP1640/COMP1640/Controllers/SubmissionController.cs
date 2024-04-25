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

		public SubmissionController(UmcsContext db, IToastNotification toast, IFile file, IEmail email)
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

			var term = _db.Terms.FirstOrDefault(t => t.Status != null && t.Status.Equals("Active"));
			if (term == null)
			{
				return NotFound();
			}

			ViewBag.Faculty = faculty.FacultyName;
			ViewBag.Term = term.TermName;
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

			var coor = _db.Users.FirstOrDefault(x => x.RoleId == 3 && x.FacultyId == user.FacultyId);
			if (coor == null)
			{
				return NotFound("Coordinator not found.");
			}

			var term = await _db.Terms.FirstOrDefaultAsync(t => t.Status == "Active");
			if (term == null)
			{
				return NotFound("Active term not found.");
			}

			if (ModelState.IsValid)
			{
                article.Content = "";
                article.ImagePath = "";
                article.UserId = user.UserId;
                article.SubmissionDate = DateTime.Now;
                article.TermId = term.TermId;
                article.FacultyId = user.FacultyId;
                article.Status = "Pending";

                _db.Articles.Add(article);

                await _db.SaveChangesAsync();

                article.Content = await Upload(file, article.ArticleId.ToString());
                article.ImagePath = await Upload(img, article.ArticleId.ToString());
                _db.Entry(article).State = EntityState.Modified;

                await _db.SaveChangesAsync();

                await SendMail(coor.Username, user.Username, article.Title, article.ArticleId, article.SubmissionDate);
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
			ViewBag.Term = term.TermName;

			return View(article);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditSubmission(int id, Article article, IFormFile file, IFormFile img)
		{
			if (id != article.ArticleId)
			{
				return NotFound();
			}

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

            var existingArticle = await _db.Articles.FindAsync(id);
            if (existingArticle == null)
            {
				return NotFound("Article not found.");
            }

            if (file != null)
            {
                existingArticle.Content = await Upload(file, existingArticle.ArticleId.ToString());
            }

            if (img != null)
            {
                existingArticle.ImagePath = await Upload(img, existingArticle.ArticleId.ToString());
            }

            ModelState.Remove(nameof(img));
			ModelState.Remove(nameof(file));

            if (ModelState.IsValid)
			{
				try
				{
					existingArticle.Title = article.Title;
					existingArticle.SubmissionDate = DateTime.Now;
					existingArticle.TermId = term.TermId;
					existingArticle.FacultyId = user.FacultyId;
					existingArticle.Status = "Pending";

					_db.Entry(existingArticle).State = EntityState.Modified;
					await _db.SaveChangesAsync();

					_toast.AddSuccessToastMessage("Article updated successfully!");
					return RedirectToAction("Index", "Home");
				}
				catch (DbUpdateConcurrencyException)
				{
					return NotFound();
				}
			}

			_toast.AddErrorToastMessage("Failed to update article!");
			return View(article);
		}

		public IActionResult ContributeDetail(int id)
		{
			HttpContext.Session.SetInt32("id", id);

			var article = _db.Articles.Include(a => a.User)
									  .Include(a => a.Comments)
									  .FirstOrDefault(a => a.ArticleId == id);
			if (article == null)
			{
				return NotFound();
			}

			var student = _db.Users.FirstOrDefault(s => s.UserId == article.UserId);
			if (student == null)
			{
				return NotFound();
			}
			ViewBag.StudentAvatar = student.Avatar;

			var comment = _db.Comments.Include(c => c.User).FirstOrDefault(c => c.ArticleId == article.ArticleId);
			if (comment != null)
			{
				ViewBag.Comment = comment.CommentContent;
				ViewBag.CommentDate = comment.CommentDate;

				var coordinator = _db.Users.FirstOrDefault(x => x.UserId == comment.UserId && x.RoleId == 3);
				if (coordinator == null)
				{
					return NotFound();
				}

				ViewBag.Coordinator = coordinator.Username;
				ViewBag.Avatar = coordinator.Avatar;
			}

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

        public async Task SendMail(string manager, string name, string title, int id, DateTime? date)
        {
            var email = "id22052003@gmail.com";
            var subject = "Contribute announce";
            var message =
                $"<h2>New Submission Received</h2>" +
                $"<p>Dear {manager},<p>" +
                $"<p>We have received a new submission from {name}.</p>" +
                $"<p>Here are the details of the submission:<p>" +
                $"<ul>" +
                $"<li style='list-style-type:None'><b>Submission Title:</b>{title}</li>" +
                $"<li style='list-style-type:None'><b>Submitted On:</b>{date}</li>" +
                $"</ul>" +
                $"<p>Please review the submission at your earliest convenience.</p>" +
                $"<p>If you have any questions, feel free to contact us at thaiphgcs210953@fpt.edu.vn.</p>" +
                $"<p>Best regards!</p>";
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

		public Article Article(int? id)
		{
			var article = _db.Articles.Where(x => x.ArticleId == id).FirstOrDefault();
			if (article == null)
			{
				throw new Exception();
			}
			return article;
		}

		public string ContentPath()
		{
			var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "UploadedFiles"));
			return path;
		}

		public string OutPath()
		{
			var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "OutPut"));
			if (!Directory.Exists(path))
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

			string filePath = Path.Combine(ContentPath(), article.ArticleId.ToString(), article.Content);
			string outFilePath = Path.Combine(OutPath(), article.Content.Split(".")[0] + ".pdf");

			using (Viewer viewer = new Viewer(filePath))
			{
				PdfViewOptions options = new PdfViewOptions(outFilePath);
				viewer.View(options);
			}

			FileStream fs = new FileStream(outFilePath, FileMode.Open, FileAccess.Read);
			FileStreamResult fsr = new FileStreamResult(fs, "application/pdf");
			return fsr;
		}
	}
}
