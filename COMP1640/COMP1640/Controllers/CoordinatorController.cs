using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace COMP1640.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ILogger<CoordinatorController> _logger;
        private readonly UmcsContext _umcs;

        public CoordinatorController(ILogger<CoordinatorController> logger, UmcsContext umcs)
        {
            _logger = logger;
            _umcs = umcs;
        }

        public IActionResult StuIndex(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstStudent = _umcs.Users.Where(stu => stu.RoleId == 4).AsNoTracking().OrderBy(stu => stu.Username);
            PagedList<User> model = new PagedList<User>(lstStudent, pageNumber, pageSize);
            return View(model);
        }

        public IActionResult GuestIndex()
        {
            var guests = _umcs.Users.Where(guest => guest.RoleId == 10).AsNoTracking().OrderBy(guest => guest.Username);
            return View(guests);
        }

        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddStudent(int userId)
        {
            return View();
        }

        public IActionResult AddGuest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddGuest(int userId)
        {
            return View();
        }
    }
}
