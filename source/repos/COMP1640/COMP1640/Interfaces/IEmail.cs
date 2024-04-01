namespace COMP1640.Interfaces
{
    public interface IEmail
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
