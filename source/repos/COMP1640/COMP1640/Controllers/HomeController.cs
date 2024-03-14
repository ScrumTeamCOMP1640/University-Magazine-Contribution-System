using System.Diagnostics;
using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;

namespace COMP1640.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UmcsContext _umcs;
		public HomeController(ILogger<HomeController> logger, UmcsContext umcs)
		{
			_logger = logger;
			_umcs = umcs;
		}

		
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
