using System.Net.Mail;

namespace Honeypot.Services
{
    public interface IEmailService
    {
        void Send(MailMessage message);
    }
}
