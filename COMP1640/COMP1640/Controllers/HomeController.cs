using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Controllers
{
    public class HomeController : Controller
    {
        private readonly UmcsContext _db;

        public HomeController(UmcsContext db)
        {
            _db = db;
        }

        public IActionResult Index(int? page)
        {
			var user = GetUser();
			if (user == null)
			{
				return NotFound();
			}

			var role = GetRole();
			if (role == null)
			{
				return NotFound();
			}

			var faculties = GetFaculties(user, role);
            var terms = _db.Terms.OrderBy(t => t.Status);
			var articlesQuery = GetFilteredArticlesQuery(user, role);

            int pageSize = 8;
            int pageNumber = page ?? 1;
            IPagedList<Article> pagedList = articlesQuery.ToPagedList(pageNumber, pageSize);

            ViewBag.Faculties = faculties;
            ViewBag.Terms = terms;

            return View(pagedList);
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

			var comment = _db.Comments.Include(c => c.User).FirstOrDefault(c => c.ArticleId == article.ArticleId);
			if (comment != null)
			{
				ViewBag.Comment = comment.CommentContent;

				var coordinator = _db.Users.FirstOrDefault(x => x.UserId == comment.UserId && x.RoleId == 3);
				if (coordinator == null)
				{
					return NotFound();
				}

				ViewBag.Coordinator = coordinator.Username;
			}

			return View(article);
		}

		public IActionResult GetArticles(int? page, int facultyId, int termId, string searchQuery)
        {
			var user = GetUser();
			if (user == null)
			{
				return NotFound();
			}

			var role = GetRole();
			if (role == null)
			{
				return NotFound();
			}

			var articlesQuery = GetFilteredArticlesQuery(user, role);

			if (facultyId != 1)
            {
                articlesQuery = articlesQuery.Where(a => a.FacultyId == facultyId);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                articlesQuery = articlesQuery.Where(a => a.Title.Contains(searchQuery) || 
                    (a.User != null && a.User.Username != null && a.User.Username.Contains(searchQuery)));
            }

            articlesQuery = articlesQuery.Where(a => a.TermId == termId);

			int pageSize = 8;
            int pageNumber = page ?? 1;
            IPagedList<Article> pagedList = articlesQuery.ToPagedList(pageNumber, pageSize);

            return PartialView("_ArticlesPartial", pagedList);
        }


        public IActionResult GetTermDates(int termId)
		{
			var term = _db.Terms.FirstOrDefault(t => t.TermId == termId);
			if (term != null)
			{
				return Json(new { startDate = term.StartDate.ToString("dd/MM/yyyy"), endDate = term.EndDate.ToString("dd/MM/yyyy"), status = term.Status });
			}
			else
			{
				return Json(new { startDate = "", endDate = "", status = "" });
			}
		}

		private User? GetUser()
		{
			var userEmail = HttpContext.Session.GetString("Email");
			if (string.IsNullOrEmpty(userEmail))
            {
				return null;
            }
            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
			return user;
		}

		private string? GetRole()
		{
			return HttpContext.Session.GetString("Role");
		}

		private IQueryable<Faculty> GetFaculties(User user, string role)
		{
			IQueryable<Faculty> faculties = _db.Faculties.OrderBy(f => f.FacultyId);
			if (role != "Marketing Manager")
				faculties = faculties.Where(f => f.FacultyId == user.FacultyId);
			return faculties;
		}

		private IQueryable<Article> GetFilteredArticlesQuery(User user, string role)
		{
			IQueryable<Article> articlesQuery = _db.Articles
				.Include(a => a.User)
				.OrderBy(a => a.Title);

			switch (role)
			{
				case "Marketing Manager":
					articlesQuery = articlesQuery.Where(a => a.Status == "Published");
					break;
				case "Guest":
					articlesQuery = articlesQuery.Where(a => a.Status == "Published" && a.FacultyId == user.FacultyId);
					break;
				case "Marketing Coordinator":
				case "Student":
					articlesQuery = articlesQuery.Where(a => a.FacultyId == user.FacultyId);
					break;
			}

			return articlesQuery;
		}
	}
}
