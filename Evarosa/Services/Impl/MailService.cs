using System.Net.Mail;
using System.Net;
using System.Text;

namespace Evarosa.Services.Impl
{
    public class MailService : IMailService
    {
        public async Task<string> SendEmailAsync(string smtpmail, string subject, string body, string emailTo, string emailFrom, string username, string password, string emailName = null, string bcc = null, string replyTo = null)
        {
            try
            {
                if (emailName == null)
                {
                    emailName = "Vico Register";
                }

                string host = "smtp.gmail.com";
                int port = 587;
                switch (smtpmail)
                {
                    case "gmail":
                        host = "smtp.gmail.com";
                        break;
                    case "yahoo":
                        host = "smtp.mail.yahoo.com";
                        break;
                    case "live":
                        host = "smtp.live.com";
                        break;
                    case "amazon":
                        host = "email-smtp.us-east-1.amazonaws.com";
                        break;
                    case "zoho":
                        host = "smtp.zoho.com";
                        break;
                    case "yandex":
                        host = "smtp.yandex.com";
                        break;
                }

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress(emailFrom, emailName);
                    message.To.Add(emailTo);
                    if (replyTo != null)
                    {
                        message.ReplyToList.Add(replyTo);
                    }

                    if (bcc != null)
                    {
                        message.Bcc.Add(bcc);
                    }

                    message.Subject = subject;
                    message.SubjectEncoding = Encoding.UTF8;
                    message.Body = body;
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;
                    using (SmtpClient smtpClient = new SmtpClient(host, port))
                    {
                        smtpClient.Credentials = new NetworkCredential(username, password);
                        smtpClient.EnableSsl = true;
                        await smtpClient.SendMailAsync(message);
                    }
                }

                return "Send Successfull";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
