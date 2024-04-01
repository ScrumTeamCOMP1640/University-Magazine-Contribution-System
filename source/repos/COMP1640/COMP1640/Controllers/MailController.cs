using COMP1640.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace COMP1640.Controllers
{
    public class MailController : Controller
    {
        private readonly IEmail _email;

        public MailController(IEmail email)
        {
            _email = email;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Send(string message)
        {
            var receiver = "phamductin120603@gmail.com";
            var subject = "Top sercet link";

            await _email.SendEmailAsync(receiver, subject, message);

            return RedirectToAction("Index");
        }
    }
}
