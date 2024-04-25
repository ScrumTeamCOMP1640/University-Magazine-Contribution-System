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

        /*public IActionResult GetContributionsData()
        {
            var data = _db.Articles
                .Include(a => a.Term)
                .Include(a => a.Faculty)
                .Where(a => a.Status == "Approved")
                .GroupBy(a => new { a.Term.TermName, a.Faculty.FacultyName })
                .Select(g => new
                {
                    Term = g.Key.TermName,
                    Faculty = g.Key.FacultyName,
                    Count = g.Count()
                })
                .ToList();

            var terms = data.Select(d => d.Term).Distinct().ToList();
            var faculties = data.Select(d => d.Faculty).Distinct().ToList();

            var contributions = new List<Dictionary<string, int>>();
            foreach (var term in terms)
            {
                var termData = new Dictionary<string, int>();
                foreach (var faculty in faculties)
                {
                    var count = data.FirstOrDefault(d => d.Term == term && d.Faculty == faculty)?.Count ?? 0;
                    termData.Add(faculty, count);
                }
                contributions.Add(termData);
            }

            return Json(new { terms, faculties, contributions });
        }*/

        public IActionResult Chart()
        {
            var terms = _db.Terms.OrderBy(t => t.Status);
            ViewBag.Terms = terms;
            return View();
        }

        public IActionResult GetContributorsByFacultyForTerm(int termId)
        {
            // Query the database to get the necessary data
            var contributorsByFaculty = _db.Articles
                .Where(a => a.TermId == termId && a.Status == "Approved")
                .GroupBy(a => a.Faculty.FacultyName)
                .Select(g => new
                {
                    FacultyName = g.Key,
                    ContributorCount = g.Count()
                })
                .ToList();

            if (contributorsByFaculty.Any())
            {
                var faculties = contributorsByFaculty.Select(item => item.FacultyName).ToList();
                var contributorCounts = contributorsByFaculty.Select(item => item.ContributorCount).ToList();

                return Json(new { faculties, contributorCounts });
            }
            else
            {
                return Json(new { faculties = new List<string>(), contributorCounts = new List<int>() });
            }
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
                .OrderByDescending(a => a.SubmissionDate);

            switch (role)
            {
                case "Marketing Manager":
                    articlesQuery = articlesQuery.Where(a => a.Status == "Approved");
                    break;
                case "Guest":
                    articlesQuery = articlesQuery.Where(a => a.Status == "Approved" && a.FacultyId == user.FacultyId);
                    break;
                case "Marketing Coordinator":
                    articlesQuery = articlesQuery.Where(a => a.FacultyId == user.FacultyId);
                    break;
                case "Student":
                    articlesQuery = articlesQuery.Where(a => a.UserId == user.UserId || (a.Status == "Approved" && a.FacultyId == user.FacultyId));
                    break;
            }

            return articlesQuery;
        }
    }
}
