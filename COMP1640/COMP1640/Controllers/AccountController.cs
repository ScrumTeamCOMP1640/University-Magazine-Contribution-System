using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web.Helpers;
using NToastNotify;
using COMP1640.ViewModels;

namespace COMP1640.Controllers
{
    public class AccountController : Controller
    {
        private readonly UmcsContext _db;
        private readonly IToastNotification _toast;

        public AccountController(UmcsContext db, IToastNotification toast)
        {
            _db = db;
            _toast = toast;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                var loginVM = new LoginViewModel();
                return View(loginVM);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel loginVM)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var client = _db.Users.FirstOrDefault(c => c.Email == loginVM.Email);

            if (client != null)
            {
                if (loginVM.Password != null && Crypto.VerifyHashedPassword(client.Password, loginVM.Password))
                {
                    HttpContext.Session.SetString("Username", client.Username);
                    HttpContext.Session.SetString("Email", client.Email);
                    HttpContext.Session.SetString("Avatar", client.Avatar ?? "");

                    string role = "";
                    switch (client.RoleId)
                    {
                        case 1:
                            role = "Administrator";
                            return RedirectToAction("Index", "Home", new { area = "Admin" });
                        case 2:
                            role = "Marketing Manager";
                            break;
                        case 3:
                            role = "Marketing Coordinator";
                            break;
                        case 4:
                            role = "Student";
                            break;
                        case 5:
                            role = "Guest";
                            break;
                    }
                    HttpContext.Session.SetString("Role", role);
                    return RedirectToAction("Index", "Home");
                }
            }

            _toast.AddErrorToastMessage("Invalid email or password!");
            return View(loginVM);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

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

            originalUser.Username = profile.Username;
            originalUser.Phone = profile.Phone;
            originalUser.Address = profile.Address;

            HttpContext.Session.SetString("Username", profile.Username);
            _db.SaveChanges();
            _toast.AddSuccessToastMessage("Information updated successfully!");
            return RedirectToAction("Profile");
        }

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
                Username = user.Username,
                Phone = user.Phone,
                Address = user.Address
            };

            return View(profileViewModel);
        }

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
