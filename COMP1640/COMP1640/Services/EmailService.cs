using COMP1640.Interfaces;
using System.Net;
using System.Net.Mail;

namespace COMP1640.Services
{
    public class EmailService : IEmail
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "plsdontreply5@gmail.com";
            var password = "lagz zlfw iull btyq";
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            return client.SendMailAsync(new MailMessage(from: mail,to: email, subject, message));
        }
    }
}
