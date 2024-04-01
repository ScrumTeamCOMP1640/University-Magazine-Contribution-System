using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Semester")]
    public class SemesterController : Controller
    {
        private readonly UmcsContext _umcs;

        public SemesterController(UmcsContext umcs)
        {
            _umcs = umcs;
        }

        [Route("")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstSemesters = _umcs.Semesters.AsNoTracking().OrderBy(_umcs => _umcs.SemesterId);
            PagedList<Semester> model = new PagedList<Semester>(lstSemesters, pageNumber, pageSize);
            ViewBag.Roles = _umcs.Roles.ToList();
            return View(model);
        }

        [Route("Add")]
        public IActionResult Add()
        {
            return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Semester semester)
        {
            if (ModelState.IsValid)
            {
                _umcs.Semesters.Add(semester);
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(semester);
        }
    }
}
