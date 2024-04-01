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
		private readonly IFile _file;
        private readonly IEmail _email;

		public HomeController(ILogger<HomeController> logger, UmcsContext umcs, IFile file, IEmail email)
		{
			_logger = logger;
			_umcs = umcs;
			_file = file;
            _email = email;
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
                try
                {
                    if (await _file.UploadFile(file))
                    {
                        article.Content = file.FileName;
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
                
                try
                {
                    if (await _file.UploadFile(img))
                    {
                        article.ImagePath = img.FileName;
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

                if (student == null)
                {
                    return NotFound();
                }

                if (coordinator == null)
                {
                    return NotFound();
                }

                article.UserId = student.UserId;
                article.SubmissionDate = DateTime.Now;
				article.Status = "Pending";
                _umcs.Add(article);
                await _umcs.SaveChangesAsync();

                var receiver = "id22052003@gmail.com";
                var subject = "New contriute is waitting for review";
                var message = coordinator.Username;

                await _email.SendEmailAsync(receiver, subject, message);
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

		public IActionResult ContributeDetail(int? id)
		{
            var article = _umcs.Articles.FirstOrDefault(a => a.ArticleId == id);
            ViewBag.Users = _umcs.Users.ToList();
			return View(article);
		}

		

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
