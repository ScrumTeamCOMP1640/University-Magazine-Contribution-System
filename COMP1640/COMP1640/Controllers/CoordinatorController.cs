using COMP1640.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Mono.TextTemplating;
using NuGet.Versioning;

namespace COMP1640.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly ILogger<CoordinatorController> _logger;
        private readonly UmcsContext _context;
        
        public CoordinatorController(ILogger<CoordinatorController> logger, UmcsContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Comment(int? id, string comment, string status)
        {
            var article = _context.Articles.FirstOrDefault(x => x.ArticleId == id);
            if (article == null)
            {
                throw new Exception();
            }

            var user = _context.Users.FirstOrDefault(x => x.Username.Equals(HttpContext.Session.GetString("Username")));
            if (user == null)
            {
                throw new Exception();
            }

            var coor = _context.Users.FirstOrDefault(x => x.FacultyId == user.FacultyId && x.RoleId == 3);
            if (coor == null)
            {
                throw new Exception();
            }

            if (ModelState.IsValid)
            {
                article.Status = status;
                _context.Entry(article).State = EntityState.Modified;
                _context.SaveChanges();

                _context.Add(new Comment
                {
                    ArticleId = id,
                    CommentContent = comment,
                    CommentDate = DateTime.Now,
                    UserId = coor.UserId
                });
                _context.SaveChanges();
                ViewData["Message"] = "Comment has been sended";
                return RedirectToAction("ContributeDetail", "Submission", new {id = id});
            }
            ViewData["Message"] = "Comment cannot send";
            return RedirectToAction("ContributeDetail", "Submission", new { id = id });
        }
    }
}
