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

            var curr = new Term();
            foreach (var term in _umcs.Terms.ToList())
            {
                if (term.EndDate > DateTime.Now && term.StartDate < DateTime.Now)
                {
                    curr = term;
                    ViewBag.CurrTerms = term;
                }
            }
            ViewBag.PreTerms = _umcs.Terms.Where(x => x.EndDate.AddDays(1) == curr.StartDate).FirstOrDefault();
            HttpContext.Session.SetString("Term", curr.TermName);
            ViewBag.Users = _umcs.Users.ToList();
            return View(model);
        }  
    }
}
