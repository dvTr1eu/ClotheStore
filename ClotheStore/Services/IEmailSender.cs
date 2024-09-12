namespace ClotheStore.Services
{
    public interface IEmailSender
    {
        Task SendMailAsync(string toEmail, string subject, string message);
    }
}
