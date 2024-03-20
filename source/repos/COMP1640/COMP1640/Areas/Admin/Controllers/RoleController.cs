using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    public class RoleController : Controller
    {
        UmcsContext _umcs = new UmcsContext();
        public IActionResult Index()
        {
            var roles = _umcs.Roles.ToList();
            return View(roles);
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
        public IActionResult Edit(int id)
        {

            return View(_umcs.Roles.FirstOrDefault(m => m.RoleId.Equals(id)));
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Role role)
        {
            if (ModelState.IsValid)
            {
                _umcs.Roles.Update(role);
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

            if (role.Users.Count > 0)
            {
                return BadRequest();
            }

            _umcs.Roles.Remove(role);
            _umcs.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
