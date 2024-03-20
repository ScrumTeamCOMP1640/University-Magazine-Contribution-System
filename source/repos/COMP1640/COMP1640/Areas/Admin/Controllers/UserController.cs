using Microsoft.AspNetCore.Mvc;
using COMP1640.Models;
using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        UmcsContext _umcs = new UmcsContext();

        [Route("")]
        public IActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value ;
            var lstUser = _umcs.Users.AsNoTracking().OrderBy(_umcs => _umcs.UserId);
            PagedList<User> model = new PagedList<User>(lstUser, pageNumber, pageSize);
            return View(model);
        }

        [Route("Add")]
        public IActionResult Add()
        {
            ViewBag.Roles = _umcs.Roles.ToList();
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
                user.Avatar = "avatar1.jpg";
                _umcs.Users.Add(user);
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int? id)
        {  
            if (id == null)
            {
                return NotFound();
            }

            var user = _umcs.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = _umcs.Roles.ToList();
            
            return View(user);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if(ModelState.IsValid)
            {
                _umcs.Entry(user).State = EntityState.Modified;
                _umcs.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        [Route("Delete")] 
        public IActionResult Delete(int id)
        {
            var user = _umcs.Users.FirstOrDefault(m => m.UserId.Equals(id));

            if (user == null)
            {
                return NotFound();
            }

            _umcs.Users.Remove(user);
            _umcs.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
