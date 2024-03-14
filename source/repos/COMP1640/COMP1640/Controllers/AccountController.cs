using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;

namespace COMP1640.Controllers
{
    public class AccountController : Controller
    {
        private readonly UmcsContext _umcs;

        public AccountController(UmcsContext umcs)
        {
            _umcs = umcs;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Email") == null)
            {
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

            var customer = _umcs.Users.FirstOrDefault(x => x.Username.Equals(user1.Username));

            if (customer != null)
            {
                if (user1.Password != null && user1.Password.Equals(customer.Password)/*Crypto.VerifyHashedPassword(customer.Password, user1.Password)*/)
                {
                    HttpContext.Session.SetString("Email", customer.Email);
                    HttpContext.Session.SetString("Username", customer.Username);

                    /*if (IsAdmin(customer.RoleId))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        HttpContext.Session.SetString("Role", "Student");
                        return RedirectToAction("Index", "Home");
                    }*/

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
                        case 5:
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
            return RedirectToAction("Index", "Home");
        }
    }
}
