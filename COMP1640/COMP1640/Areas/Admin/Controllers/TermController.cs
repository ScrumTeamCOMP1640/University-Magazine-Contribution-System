using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Term")]
    public class TermController : Controller
    {
        UmcsContext _umcs = new UmcsContext();

        [Route("")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstTerm = _umcs.Terms.OrderBy(_umcs => _umcs.StartDate);
            PagedList<Term> model = new PagedList<Term>(lstTerm, pageNumber, pageSize);
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
        public IActionResult Add(Term term)
        {
            if (ModelState.IsValid)
            {
                _umcs.Add(term);
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(term);
        }

        [Route("Edit")]
        public IActionResult Edit(int? id)
        {
            var term = _umcs.Terms.FirstOrDefault(x => x.TermId == id);
            return View(term);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Term term)
        {
            if (ModelState.IsValid)
            {
                _umcs.Entry(term).State = EntityState.Modified;
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(term);
        }

        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var term = _umcs.Terms.FirstOrDefault(m => m.TermId.Equals(id));

            if (term == null)
            {
                return NotFound();
            }

            _umcs.Terms.Remove(term);
            _umcs.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
