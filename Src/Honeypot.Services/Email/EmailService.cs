using System.Net.Mail;

namespace Honeypot.Services
{
    public class EmailService : IEmailService
    {
        public void Send(MailMessage message)
        {
            var smtp = new SmtpClient();
            smtp.Send(message);
        }
    }
}
