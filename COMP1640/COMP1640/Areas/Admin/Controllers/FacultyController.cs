using Azure;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            ViewBag.Users = _umcs.Users.ToList();
            return View(model);
        }

        [Route("Add")]
        public IActionResult Add()
        {
            ViewBag.Coordinator = _umcs.Users.Where(x => x.RoleId == 3).ToList();
            ViewBag.Guest = _umcs.Users.Where(x => x.RoleId == 10).ToList();
            return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Faculty faculty, int coorId, int guestId)
        {
            if (ModelState.IsValid)
            {
                _umcs.Faculties.Add(faculty);
                _umcs.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        [Route("Edit")]
        public ActionResult Edit(int id)
        {
            var faculty = _umcs.Faculties.Find(id);
            ViewBag.Users = _umcs.Users.ToList();
            return View(faculty);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Faculty faculty, int coorId, int guestId)
        {
            if (ModelState.IsValid)
            {
                SetFaculty(coorId, faculty.FacultyId);
                SetFaculty(guestId, faculty.FacultyId);

                _umcs.Entry(faculty).State = EntityState.Modified;
                _umcs.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var faculty = _umcs.Faculties.FirstOrDefault(f => f.FacultyId.Equals(id));
            if (faculty == null)
            {
                return NotFound();
            }

            _umcs.Faculties.Remove(faculty);
            _umcs.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public void SetFaculty (int? userId, int? facultyId)
        {
            var user = _umcs.Users.Where(x => x.UserId == userId).FirstOrDefault();
            if (user == null)
            {
                return;
            }
            user.FacultyId = facultyId;
            _umcs.Update(user);
            _umcs.SaveChanges();
        }
    }
}
