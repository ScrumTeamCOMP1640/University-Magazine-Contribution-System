using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;
using System.Web.Helpers;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using NToastNotify;
using Microsoft.AspNetCore.Mvc.Rendering;
using COMP1640.Interfaces;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IEmail _email;
        private readonly IToastNotification _toast;

        public UserController(UmcsContext db, IEmail email, IToastNotification toast)
        {
            _db = db;
            _email = email;
            _toast = toast;
        }

        [HttpGet]
        public IActionResult Index(int? page)
        {
            var userEmail = HttpContext.Session.GetString("Email");
            var users = _db.Users.Include(u => u.Role).Where(u => u.Email != userEmail).OrderBy(u => u.RoleId);
            int pageSize = 8;
            int pageNumber = page ?? 1;
            IPagedList<User> pagedList = users.ToPagedList(pageNumber, pageSize);
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
        public async Task<IActionResult> Add(User user)
        {
            if (ModelState.IsValid)
            {
                Random random = new Random();
                var pass = random.Next(100000, 999999);

                user.Password = Crypto.HashPassword(pass.ToString());
                user.Avatar = "user_avatar.png";
                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                if (user.RoleId == 4)
                {
                    await SendMail(user.Email, pass.ToString());
                }
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

        public async Task SendMail(string mail, string pass)
        {
            var email = "id22052003@gmail.com";
            var subject = "Welcome Email";
            var message =
                $"<h2>Welcome to Our School</h2>" +
                $"<p>Dear id22052003@gmail.com<p>Here are your login details:</p>" +
                $"<ul>" +
                $"<li style='list-style-type:None'><b>Email:</b>{mail}</li>" +
                $"<li style='list-style-type:None'><b>Password:</b>{pass}</li>" +
                $"</ul>" +
                $"<p>Please change your password after your first login.</p>" +
                $"<p>If you have any questions, feel free to contact us at thaiphgcs210953@fpt.edu.vn.</p>" +
                $"<p>Best regards,</p>" +
                $"<p>Hoang Thai</p>";
            await _email.SendEmailAsync(email, subject, message);
        }
    }
}
