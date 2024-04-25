using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;
using COMP1640.ViewModels;
using System.Web.Helpers;
using NToastNotify;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/Home")]
    public class HomeController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IToastNotification _toast;

        public HomeController(UmcsContext db, IToastNotification toast)
        {
            _db = db;
            _toast = toast;
        }

        [Route("Index")]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "User");
        }

        [Route("Profile")]
        public IActionResult Profile()
        {
            var userEmail = HttpContext.Session.GetString("Email");

            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var profileViewModel = new ProfileViewModel
            {
                Username = user.Username,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(profileViewModel);
        }

        [Route("Profile")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(ProfileViewModel profile)
        {
            var userEmail = HttpContext.Session.GetString("Email");

            var originalUser = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            if (originalUser == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                _toast.AddErrorToastMessage("Failed to update information!");
                return View("Profile", profile);
            }

            originalUser.Email = profile.Email;
            originalUser.Username = profile.Username;
            originalUser.Phone = profile.Phone;
            originalUser.Address = profile.Address;

            HttpContext.Session.SetString("Username", profile.Username);
            _db.SaveChanges();
            _toast.AddSuccessToastMessage("Information updated successfully!");
            return RedirectToAction("Profile");
        }

        [Route("Password")]
        public IActionResult Pass()
        {
            var userEmail = HttpContext.Session.GetString("Email");

            var user = _db.Users.FirstOrDefault(u => u.Email == userEmail);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var profileViewModel = new ProfileViewModel
            {
                Email = user.Email,
                Username = user.Username,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(profileViewModel);
        }

        [Route("Password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePass(ProfileViewModel profile)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                if (Crypto.VerifyHashedPassword(user.Password, profile.OldPassword))
                {
                    if (profile.NewPassword == profile.ConfirmPassword)
                    {
                        user.Password = Crypto.HashPassword(profile.NewPassword);
                        _db.SaveChanges();
                        _toast.AddSuccessToastMessage("Password updated successfully!");
                        return RedirectToAction("Logout");
                    }
                    else
                    {
                        _toast.AddErrorToastMessage("New password and confirm password do not match.");
                    }
                }
                else
                {
                    _toast.AddErrorToastMessage("Incorrect old password.");
                }
            }
            else
            {
                _toast.AddErrorToastMessage("User not found.");
                return NotFound();
            }
            return RedirectToAction("Pass");
        }
    }
}
