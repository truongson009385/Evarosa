using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;

namespace Evarosa.Services.Impl
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> mailSettingsOptions, ILogger<MailService> logger)
        {
            _mailSettings = mailSettingsOptions.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(MailData mailData)
        {
            try
            {
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_mailSettings.SenderEmail, _mailSettings.SenderName);
                    mailMessage.To.Add(new MailAddress(mailData.EmailToId, mailData.EmailToName));
                    mailMessage.Subject = mailData.EmailSubject;
                    mailMessage.Body = mailData.EmailBody;
                    mailMessage.IsBodyHtml = true;

                    using (var smtpClient = new SmtpClient(_mailSettings.Server, _mailSettings.Port))
                    {
                        smtpClient.Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.Password);
                        smtpClient.EnableSsl = true;

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email to {EmailToId}", mailData.EmailToId);
                return false;
            }
        }
    }
}
