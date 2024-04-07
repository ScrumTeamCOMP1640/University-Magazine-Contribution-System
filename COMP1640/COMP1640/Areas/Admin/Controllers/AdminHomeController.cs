using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/AdminHome")]
    public class AdminHomeController : Controller
    {
        private readonly UmcsContext _context;

        public AdminHomeController(UmcsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var article = _context.Articles.ToList();
            ViewBag.Faculty = _context.Faculties.ToList();
            return View(article);
        }
    }
}
