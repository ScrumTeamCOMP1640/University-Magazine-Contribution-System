using Microsoft.AspNetCore.Mvc;

namespace COMP1640.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {        
        public IActionResult Index()
        {
            return View();
        }

        [Route("Add")]
        public IActionResult Add()
        {
            return View();
        }

        [Route("Edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
