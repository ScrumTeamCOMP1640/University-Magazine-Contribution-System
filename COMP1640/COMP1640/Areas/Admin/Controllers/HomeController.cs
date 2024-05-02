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
			return View();
		}

		public IActionResult GetUsersByFaculty()
		{
			var usersByFaculty = _db.Users
					.Where(u => u.FacultyId > 1)
					.GroupBy(u => u.Faculty.FacultyName)
					.Select(g => new
					{
						FacultyName = g.Key,
						UserCount = g.Count()
					})
					.ToList();

			if (usersByFaculty.Count == 0)
			{
				return Json(new { faculties = new List<string>(), userCounts = new List<int>() });
			}
			else
			{
				var faculties = usersByFaculty.Select(item => item.FacultyName).ToList();
				var userCounts = usersByFaculty.Select(item => item.UserCount).ToList();

				return Json(new { faculties, userCounts });
			}
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
				Email = user.Email,
				Avatar = user.Avatar,
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

			if (profile.Avatar != null)
			{
				originalUser.Avatar = profile.Avatar;
				HttpContext.Session.SetString("Avatar", profile.Avatar);
			}

			if (!string.IsNullOrEmpty(profile.OldPassword))
			{
				if (Crypto.VerifyHashedPassword(originalUser.Password, profile.OldPassword))
				{
					if (profile.NewPassword == profile.ConfirmPassword)
					{
						originalUser.Password = Crypto.HashPassword(profile.NewPassword);
						_db.SaveChanges();
						_toast.AddSuccessToastMessage("Password updated successfully!");
						return RedirectToAction("Logout", "Account");
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

			HttpContext.Session.SetString("Username", profile.Username);
			_db.SaveChanges();
			_toast.AddSuccessToastMessage("Information updated successfully!");
			return RedirectToAction("Profile");
		}
	}
}
