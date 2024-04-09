using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;

namespace COMP1640.Controllers
{
    public class AccountController : Controller
    {
        UmcsContext _umcs = new UmcsContext();
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
                ViewBag.Role = _umcs.Roles.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user1)
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            var customer = _umcs.Users.FirstOrDefault(u => u.Email == user1.Email);

            if (customer != null)
            {
                if (user1.Password != null && Crypto.VerifyHashedPassword(customer.Password, user1.Password))
                {
                    if (customer.Username != null)
                    {
                        HttpContext.Session.SetString("Username", customer.Username);
                    }

                    if (customer.Email != null)
                    {
                        HttpContext.Session.SetString("Email", customer.Email);
                    }

                    if (customer.Avatar != null)
                    {
                        HttpContext.Session.SetString("Avatar", customer.Avatar);
                    }

                    switch (customer.RoleId)
                    {
                        case 1:
                            HttpContext.Session.SetString("Role", "Administrator");
                            return RedirectToAction("Index", "AdminHome", new { area = "Admin" });
                        case 2:
                            HttpContext.Session.SetString("Role", "Marketing Manager");
                            return RedirectToAction("Index", "Home");
                        case 3:
                            HttpContext.Session.SetString("Role", "Marketing Coordinator");
                            return RedirectToAction("Index", "Home");
                        case 4:
                            HttpContext.Session.SetString("Role", "Student");
                            return RedirectToAction("Index", "Home");
                        case 10:
                            HttpContext.Session.SetString("Role", "Guest");
                            return RedirectToAction("Index", "Home");
                    }
                }
            }

            TempData["LoginError"] = "Invalid email or password.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Login");
        }

        public IActionResult UpdateInfo()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _umcs.Users.FirstOrDefault(u => u.Email == email);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(User user)
        {
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    _umcs.Entry(user).State = EntityState.Modified;
                    _umcs.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(user);
        }

        public IActionResult UpdatePass()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _umcs.Users.FirstOrDefault(u => u.Email == email);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePass(string password, string newPass, string confirmNewPass)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _umcs.Users.FirstOrDefault(u => u.Email == email);
            if(user != null)
            {
                if (Crypto.VerifyHashedPassword(user.Password, password))
                {
                    if (newPass == confirmNewPass)
                    {
                        user.Password = Crypto.HashPassword(newPass);
                        _umcs.SaveChanges();
                        
                    }
                    else
                    {
                        TempData["UpdatePassError"] = "Old password is incorrect.";
                        return View();
                    }
                }
            }
            return Logout();
        }

    }
}
