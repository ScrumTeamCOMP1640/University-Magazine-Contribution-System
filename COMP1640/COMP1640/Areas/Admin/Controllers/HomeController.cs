using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Home")]
    public class HomeController : Controller
    {
        private readonly UmcsContext _db;

        public HomeController(UmcsContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var article = _db.Articles.ToList();
            ViewBag.Faculty = _db.Faculties.ToList();
            return View(article);
        }
    }
}
