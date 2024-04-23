using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using NToastNotify;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    public class RoleController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IToastNotification _toast;

        public RoleController(UmcsContext db, IToastNotification toast)
        {
            _db = db;
            _toast = toast;
        }

        public IActionResult Index(int? page)
        {
            var roles = _db.Roles.OrderBy(r => r.RoleId);
            int pageSize = 8;
            int pageNumber = page ?? 1;
            IPagedList<Role> pagedList = roles.ToPagedList(pageNumber, pageSize);
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
        public IActionResult Add(Role role)
        {
            if (ModelState.IsValid)
            {
                _db.Roles.Add(role);
                _db.SaveChanges();
                _toast.AddSuccessToastMessage("Role created successfully!");
                return RedirectToAction("Index");
            }
            return View(role);
        }

        [Route("Edit/{id}")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _toast.AddErrorToastMessage("Role ID not provided!");
                return NotFound();
            }

            var role = _db.Roles.Find(id);

            if (role == null)
            {
                _toast.AddErrorToastMessage("Role not found!");
                return NotFound();
            }
            return View(role);
        }

        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("RoleId, RoleName")] Role role)
        {
            if (id != role.RoleId)
            {
                _toast.AddErrorToastMessage("Invalid role ID!");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(role).State = EntityState.Modified;
                    _db.SaveChanges();
                    _toast.AddSuccessToastMessage("Role updated successfully!");
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    _toast.AddErrorToastMessage("Failed to update role!");
                    throw;
                }
            }
            return View(role);
        }

        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var role = _db.Roles.Find(id);
            if (role == null)
            {
                _toast.AddErrorToastMessage("Role not found!");
                return NotFound();
            }

            var hasUsers = _db.Users.Any(u => u.RoleId == role.RoleId);
            if (hasUsers)
            {
                _toast.AddErrorToastMessage("Users are associated with this role!");
                return RedirectToAction("Index");
            }

            _db.Roles.Remove(role);
            _db.SaveChanges();
            _toast.AddSuccessToastMessage("Role deleted successfully!");
            return RedirectToAction("Index");
        }
    }
}
