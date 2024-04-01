using Azure;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Faculty")]
    public class FacultyController : Controller
    {
        private readonly UmcsContext _umcs;

        public FacultyController(UmcsContext umcs)
        {
            _umcs = umcs;
        }

        [Route("")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstFaculties = _umcs.Faculties.AsNoTracking().OrderBy(_umcs => _umcs.FacultyId);
            PagedList<Faculty> model = new PagedList<Faculty>(lstFaculties, pageNumber, pageSize);
            ViewBag.FaUs = _umcs.FaUs.ToList();
            ViewBag.User = _umcs.Users.ToList();
            return View(model);
        }

        [Route("Add")]
        public IActionResult Add()
        {
            ViewBag.Coordinator = _umcs.Users.Where(u => u.RoleId == 3).ToList();
            return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Faculty faculty, int userId)
        {
            if (ModelState.IsValid)
            {
                _umcs.Faculties.Add(faculty);
                _umcs.SaveChanges();
                _umcs.FaUs.Add(new FaU
                {
                    FacultyId = faculty.FacultyId,
                    UserId = userId
                });
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var faculty = _umcs.Faculties.FirstOrDefault(f => f.FacultyId.Equals(id));
            var fau = _umcs.FaUs.Where(f => f.FacultyId.Equals(id)).ToList();

            if (fau == null)
            {
                return NotFound();
            }

            if (faculty == null)
            {
                return NotFound();
            }
            
            foreach (var item in fau)
            {
                _umcs.FaUs.Remove(item);
                _umcs.SaveChanges();
            }

            _umcs.Faculties.Remove(faculty);
            _umcs.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
