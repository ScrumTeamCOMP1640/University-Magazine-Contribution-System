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

        public IActionResult Index()
        {
			var user = GetUser();
			var role = GetRole();

			if (user == null || role == null)
			{
				return NotFound();
			}

            CheckArticle();

			var faculties = GetFaculties(user, role);
            var terms = _db.Terms.OrderBy(t => t.Status);

            ViewBag.Faculties = faculties;
            ViewBag.Terms = terms;

            return View();
        }

        public IActionResult Chart()
        {
            var academicYears = _db.Terms.GroupBy(t => t.StartDate.Year)
                                  .OrderByDescending(g => g.Key)
                                  .Select(g => g.Key)
                                  .ToList();
            ViewBag.AcademicYears = academicYears;

            var terms = _db.Terms.OrderBy(t => t.Status);
            ViewBag.Terms = terms;

            return View();
        }

        public JsonResult GetTermsByYear(int academicYear)
        {
            var terms = _db.Terms.Where(t => t.StartDate.Year == academicYear).OrderBy(t => t.Status).ToList();
            return new JsonResult(terms);
        }

        public IActionResult GetContributorsByFacultyForTerm(int termId)
        {
            var contributorsByFaculty = _db.Articles
                .Where(a => a.TermId == termId && a.Status == "Approved")
                .GroupBy(a => a.Faculty.FacultyName)
                .Select(g => new
                {
                    FacultyName = g.Key,
                    ContributorCount = g.Count()
                })
                .ToList();

            if (contributorsByFaculty.Count == 0)
            {
                return Json(new { faculties = new List<string>(), contributorCounts = new List<int>() });
            }
            else
            {
                var faculties = contributorsByFaculty.Select(item => item.FacultyName).ToList();
                var contributorCounts = contributorsByFaculty.Select(item => item.ContributorCount).ToList();

                return Json(new { faculties, contributorCounts });
            }
        }

        public IActionResult GetArticles(int? page, int facultyId, int termId, string searchQuery, string status)
        {
			var user = GetUser();
			var role = GetRole();

			if (user == null || role == null)
			{
				return NotFound();
			}

			var articlesQuery = GetFilteredArticlesQuery(user, role, facultyId, termId, searchQuery, status);

			int pageSize = 6;
			int pageNumber = page ?? 1;
			IPagedList<Article> pagedList = articlesQuery.ToPagedList(pageNumber, pageSize);

			return PartialView("_ArticlesPartial", pagedList);
		}

		public IActionResult GetTotalArticleCount(int facultyId, int termId, string searchQuery, string status)
		{
			var user = GetUser();
			var role = GetRole();

			if (user == null || role == null)
			{
				return NotFound();
			}

			var articlesQuery = GetFilteredArticlesQuery(user, role, facultyId, termId, searchQuery, status);

			int totalCount = articlesQuery.Count();

			return Json(new { totalCount });
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

		private IQueryable<Article> GetFilteredArticlesQuery(User user, string role, int facultyId, int termId, string searchQuery, string status)
		{
			IQueryable<Article> articlesQuery = _db.Articles
				.Include(a => a.User)
				.OrderByDescending(a => a.SubmissionDate);

			switch (role)
			{
				case "Marketing Manager":
					articlesQuery = articlesQuery.Where(a => a.Status == "Approved");
					break;
				case "Guest":
					articlesQuery = articlesQuery.Where(a => a.Status == "Approved" && a.FacultyId == user.FacultyId && a.TermId == termId).Take(3);
					break;
				case "Marketing Coordinator":
					articlesQuery = articlesQuery.Where(a => a.FacultyId == user.FacultyId);
					break;
				case "Student":
					articlesQuery = articlesQuery.Where(a => a.UserId == user.UserId || (a.Status == "Approved" && a.FacultyId == user.FacultyId));
					break;
			}

			if (facultyId > 1)
			{
				articlesQuery = articlesQuery.Where(a => a.FacultyId == facultyId);
			}

			if (termId > 0)
			{
				articlesQuery = articlesQuery.Where(a => a.TermId == termId);
			}

			if (!string.IsNullOrEmpty(searchQuery))
			{
				articlesQuery = articlesQuery.Where(a => a.Title.Contains(searchQuery) ||
					(a.User != null && a.User.Username != null && a.User.Username.Contains(searchQuery)));
			}

			if (!string.IsNullOrEmpty(status) && status != "All")
			{
				articlesQuery = articlesQuery.Where(a => a.Status == status);
			}

			return articlesQuery;
		}

		public void CheckArticle()
		{
			var article = _db.Articles.Where(x => x.Status != null && x.Status.Equals("Pending")).ToList();
			foreach (var item in article)
			{
				if (item.SubmissionDate != null && item.SubmissionDate.Value.AddDays(14).DayOfYear <= DateTime.Now.DayOfYear)
				{
					item.Status = "Rejected";
					_db.Entry(item).State = EntityState.Modified;
					_db.SaveChanges();
				}
			}
		}
	}
}
