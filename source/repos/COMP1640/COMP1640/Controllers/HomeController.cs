using System.Diagnostics;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;

namespace COMP1640.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View("~/Views/Home/Login.cshtml");
		}

	}
}
