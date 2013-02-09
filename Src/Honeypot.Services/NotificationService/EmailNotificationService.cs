using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Honeypot.Domain;

namespace Honeypot.Services
{
    /// <summary>
    /// Notification service that sends emails to users.
    /// </summary>
    public class EmailNotificationService : INotificationService
    {
        #region Fields

        /// <summary>
        /// An identifier within the notification template to identify which
        /// node coresponds to this notification service.
        /// </summary>
        private const string TemplateNode = "Email";

        /// <summary>
        /// Address which is used to send all notifications. Also used
        /// as the reply-to address.
        /// </summary>
        private readonly string _fromEmail;

        /// <summary>
        /// Name which is used for the email that sends all notifications.
        /// </summary>
        private readonly string _fromName;

        private readonly IEmailService _emailService;
        private readonly ILogService _logService;
        private readonly ITemplateService _templateService;

        #endregion

        public EmailNotificationService(
            ILogService logService,
            IEmailService emailService,
            ITemplateService templateService,
            string fromEmail,
            string fromName)
        {
            _fromEmail = fromEmail;
            _fromName = fromName;
            _emailService = emailService;
            _logService = logService;
            _templateService = templateService;
        }
        
        /// <summary>
        /// Send an email to the specified user
        /// </summary>
        /// <param name="recipient">Recipient of the email</param>
        /// <param name="template">Template used for email</param>
        /// <param name="messageParams">Values for placeholders used in template to replace with.</param>
        public async Task<bool> Notify(User recipient, INotificationTemplate template, object messageParams)
        {
            var message = new MailMessage(new MailAddress(_fromEmail, _fromName),
                new MailAddress(recipient.Email));

            IDictionary<string, string> templateValues = 
                _templateService.ParseTemplate(TemplateNode, template, messageParams);

            message.Subject = templateValues["Subject"];

            // Wrap the body in the parent template
            var parentEmailTemplate =
                new NotificationTemplate(template.ContainingDirectory, "Email");

            IDictionary<string, string> parentTemplate =
                _templateService.ParseTemplate("Template", parentEmailTemplate, new
                {
                    Body = templateValues["Body"]
                });

            message.Body = parentTemplate["Message"];
            message.IsBodyHtml = true;

            try
            {
                _emailService.Send(message);

                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Level = LogLevel.Info,
                    Message = string.Format("Email notification to {0} successful: {1}", message.To, message.Subject),
                    Details = message.Body
                });
            }
            catch (SmtpException ex)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Level = LogLevel.Error,
                    Message = string.Format("Email notification to {0} failed: {1}", message.To, message.Subject),
                    Details = ex.Message + " \n\n Original Email: " + message.Body
                });
            }

            await Task.Yield();
            return true;
        }

    }
}
