namespace Evarosa.Services
{
    public interface IMailService
    {
        Task<string> SendEmailAsync(string smtpmail, string subject, string body, string emailTo, string emailFrom, string username, string password, string emailName = null, string bcc = null, string replyTo = null);
    }
}
