using Azure;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    public class RoleController : Controller
    {
        UmcsContext _umcs = new UmcsContext();

        [Route("")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstRole = _umcs.Roles.AsNoTracking().OrderBy(_umcs => _umcs.RoleId);
            PagedList<Role> model = new PagedList<Role>(lstRole, pageNumber, pageSize);
            return View(model);
        }

        [Route("Add")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Role role)
        {
            if (ModelState.IsValid)
            {
                _umcs.Roles.Add(role);
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = _umcs.Roles.Find(id);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                _umcs.Entry(role).State = EntityState.Modified;
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            var role = _umcs.Roles.FirstOrDefault(m => m.RoleId.Equals(id));

            if(role == null)
            {
                return NotFound();
            }

            _umcs.Roles.Remove(role);
            _umcs.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
