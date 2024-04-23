using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using NToastNotify;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Faculty")]
    public class FacultyController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IToastNotification _toast;

        public FacultyController(UmcsContext db, IToastNotification toast)
        {
            _db = db;
            _toast = toast;
        }

        public IActionResult Index(int? page)
        {
            var faculties = _db.Faculties.OrderBy(f => f.FacultyId);
            int pageSize = 8;
            int pageNumber = page ?? 1;
            IPagedList<Faculty> pagedList = faculties.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [Route("Add")]
        public IActionResult Add()
        {
            return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _db.Faculties.Add(faculty);
                _db.SaveChanges();
                _toast.AddSuccessToastMessage("Faculty created successfully!");
                return RedirectToAction("Index");
            }
            return View(faculty);
        }

        [Route("Edit/{id}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                _toast.AddErrorToastMessage("Faculty ID not provided!");
                return NotFound();
            }

            var faculty = _db.Faculties.Find(id);

            if (faculty == null)
            {
                _toast.AddErrorToastMessage("Faculty not found!");
                return NotFound();
            }
            return View(faculty);
        }

        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("FacultyId, FacultyName")] Faculty faculty)
        {
            if (id != faculty.FacultyId)
            {
                _toast.AddErrorToastMessage("Invalid faculty ID!");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(faculty).State = EntityState.Modified;
                    _db.SaveChanges();
                    _toast.AddSuccessToastMessage("Faculty updated successfully!");
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    _toast.AddErrorToastMessage("Failed to update faculty!");
                    throw;
                }
            }
            return View(faculty);
        }

        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var faculty = _db.Faculties.Find(id);
            if (faculty == null)
            {
                _toast.AddErrorToastMessage("Faculty not found!");
                return NotFound();
            }

            var hasUsers = _db.Users.Any(u => u.FacultyId == faculty.FacultyId);
            if (hasUsers)
            {
                _toast.AddErrorToastMessage("Users are associated with this faculty!");
                return RedirectToAction("Index");
            }

            var hasArticles = _db.Articles.Any(a => a.FacultyId == faculty.FacultyId);
            if (hasArticles)
            {
                _toast.AddErrorToastMessage("Articles are associated with this faculty!");
                return RedirectToAction("Index");
            }

            _db.Faculties.Remove(faculty);
            _db.SaveChanges();
            _toast.AddSuccessToastMessage("Faculty deleted successfully!");
            return RedirectToAction("Index");
        }

    }
}
