using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;
using System.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using NToastNotify;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IToastNotification _toast;

        public UserController(UmcsContext db, IToastNotification toast)
        {
            _db = db;
            _toast = toast;
        }

        public IActionResult Index(int? page)
        {
            var users = _db.Users.OrderBy(u => u.UserId);
            int pageSize = 8;
            int pageNumber = page ?? 1;
            IPagedList<User> pagedList = users.ToPagedList(pageNumber, pageSize);
            ViewBag.Roles = _db.Roles.ToList();
            return View(pagedList);
        }

        [Route("Add")]
        public IActionResult Add()
        {
			ViewBag.RoleName = new SelectList(_db.Roles.OrderBy(r => r.RoleId).ToList(), "RoleId", "RoleName");
			ViewBag.FacultyName = new SelectList(_db.Faculties.OrderBy(f => f.FacultyId).ToList(), "FacultyId", "FacultyName");
			return View();
        }

        [Route("Add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = Crypto.HashPassword("1");
                user.Avatar = "user_avatar.png";
                _db.Users.Add(user);
                _db.SaveChanges();
                _toast.AddSuccessToastMessage("User created successfully!");
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [Route("Edit/{id}")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                _toast.AddErrorToastMessage("User ID not provided!");
                return NotFound();
            }

            var user = _db.Users.Include(u => u.Role).Include(u => u.Faculty).FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                _toast.AddErrorToastMessage("User not found!");
                return NotFound();
            }

            ViewBag.RoleName = new SelectList(_db.Roles.OrderBy(r => r.RoleId).ToList(), "RoleId", "RoleName");
            ViewBag.FacultyName = new SelectList(_db.Faculties.OrderBy(f => f.FacultyId).ToList(), "FacultyId", "FacultyName");
            return View(user);

        }

        [Route("Edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("UserId, Username, Password, Email, Avatar, RoleId, FacultyId")] User user)
        {
            if (id != user.UserId)
            {
                _toast.AddErrorToastMessage("Invalid user ID!");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                    _toast.AddSuccessToastMessage("User updated successfully!");
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    _toast.AddErrorToastMessage("Failed to update user!");
                    throw;
                }
            }
            return View(user);
        }

        [Route("Delete/{id}")] 
        public IActionResult Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                _toast.AddErrorToastMessage("User not found!");
                return NotFound();
            }

            var hasArticles = _db.Articles.Any(a => a.UserId == user.UserId);
            if (hasArticles)
            {
                _toast.AddErrorToastMessage("Articles are associated with this user!");
                return RedirectToAction("Index");
            }

            _db.Users.Remove(user);
            _db.SaveChanges();
            _toast.AddSuccessToastMessage("User deleted successfully!");
            return RedirectToAction("Index");
        }
    }
}
