using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using NToastNotify;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Term")]
    public class TermController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IToastNotification _toast;

        public TermController(UmcsContext db, IToastNotification toast)
        {
            _db = db;
            _toast = toast;
        }

        public IActionResult Index(int? page)
        {
            var terms = _db.Terms.OrderBy(t => t.StartDate);
            int pageSize = 8;
            int pageNumber = page ?? 1;
            IPagedList<Term> pagedList = terms.ToPagedList(pageNumber, pageSize);
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
        public IActionResult Add(Term term)
        {
            if (ModelState.IsValid)
            {
                _db.Terms.Add(term);
                _db.SaveChanges();
                SetTermStatus(term);
                _toast.AddSuccessToastMessage("Term created successfully!");
                return RedirectToAction("Index");
            }
            return View(term);
        }

        [Route("Edit/{id}")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _toast.AddErrorToastMessage("Term ID not provided!");
                return NotFound();
            }

            var term = _db.Terms.Find(id);

            if (term == null)
            {
                _toast.AddErrorToastMessage("Term not found!");
                return NotFound();
            }
            return View(term);
        }

        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("TermId, TermName, StartDate, EndDate, Status")] Term term)
        {
            if (id != term.TermId)
            {
                _toast.AddErrorToastMessage("Invalid term ID!");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTerm = _db.Terms.Local.FirstOrDefault(t => t.TermId == term.TermId);
                    if (existingTerm != null)
                    {
                        _db.Entry(existingTerm).State = EntityState.Detached;
                    }

                    _db.Entry(term).State = EntityState.Modified;
                    _db.SaveChanges();
                    SetTermStatus(term);
                    _toast.AddSuccessToastMessage("Term updated successfully!");
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    _toast.AddErrorToastMessage("Failed to update term!");
                    throw;
                }
            }

            return View(term);
        }

        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var term = _db.Terms.Find(id);
            if (term == null)
            {
                _toast.AddErrorToastMessage("Term not found!");
                return NotFound();
            }

            var hasArticles = _db.Articles.Any(a => a.TermId == term.TermId);
            if (hasArticles)
            {
                _toast.AddErrorToastMessage("Articles are associated with this term!");
                return RedirectToAction("Index");
            }

            _db.Terms.Remove(term);
            _db.SaveChanges();
            _toast.AddSuccessToastMessage("Term deleted successfully!");
            return RedirectToAction("Index");
        }

        private void SetTermStatus(Term term)
        {
            DateTime currentDate = DateTime.Now;

            if (term.StartDate <= currentDate && term.EndDate >= currentDate)
            {
                term.Status = "Active";
            }
            else if (term.EndDate < currentDate)
            {
                term.Status = "Inactive";
            }
            else
            {
                term.Status = "Upcoming";
            }
            _db.Entry(term).State = EntityState.Modified;
            _db.SaveChanges();
        }
    }
}
