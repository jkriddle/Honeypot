using System.Collections.Generic;
using System.Net.Mail;
using System.Xml.Linq;
using Honeypot.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Honeypot.Services.Tests
{
    [TestClass]
    public class EmailNotificationServiceTests
    {

        #region Notify

        [TestMethod]
        public void Notify_Creates_Log_On_Success()
        {
            // Arrange
            var logService = new Mock<ILogService>();
            var emailService = new Mock<IEmailService>();
            var user = new Mock<User>();
            user.SetupProperty(u => u.Email, "recipient@honeypot.com");
            var template = new Mock<INotificationTemplate>();
            template.Setup(t => t.Read()).Returns(new XElement("Email", 
                new XElement("Subject"), new XElement("Body")));
            template.Setup(t => t.ContainingDirectory).Returns("");
            var templateObj = template.Object;

            var templateService = new Mock<ITemplateService>();
            templateService.Setup(t => t.ParseTemplate(It.IsAny<string>(), 
                templateObj, 
                It.IsAny<object>()))
                .Returns(new Dictionary<string, string>()
                    {
                        {"Subject", ""},
                        {"Body", ""}
                    });

            // Parent email template
            templateService.Setup(t => t.ParseTemplate(It.IsAny<string>(), 
                It.Is<INotificationTemplate>(n => n != templateObj), 
                It.IsAny<object>()))
                .Returns(new Dictionary<string, string>()
                    {
                        {"Message", ""}
                    });

            var service = new EmailNotificationService(logService.Object, emailService.Object,
                templateService.Object,
                "test@honeypot.com", "Support");

            // Act
            service.Notify(user.Object, templateObj, null);

            // Assert
            logService.Verify(l => l.CreateLog(It.IsAny<Domain.Log>()), Times.Once());
        }

        [TestMethod]
        public void Notify_Creates_Log_On_Failure()
        {
            // Arrange
            var logService = new Mock<ILogService>();
            var emailService = new Mock<IEmailService>();
            emailService.Setup(e => e.Send(It.IsAny<MailMessage>())).Throws(new SmtpException());
            var user = new Mock<User>();
            user.SetupProperty(u => u.Email, "recipient@honeypot.com");
            var template = new Mock<INotificationTemplate>();
            template.Setup(t => t.Read()).Returns(new XElement("Email",
                new XElement("Subject"), new XElement("Body")));
            template.Setup(t => t.ContainingDirectory).Returns("");
            var templateObj = template.Object;

            var templateService = new Mock<ITemplateService>();
            templateService.Setup(t => t.ParseTemplate(It.IsAny<string>(),
                templateObj,
                It.IsAny<object>()))
                .Returns(new Dictionary<string, string>()
                    {
                        {"Subject", ""},
                        {"Body", ""}
                    });

            // Parent email template
            templateService.Setup(t => t.ParseTemplate(It.IsAny<string>(),
                It.Is<INotificationTemplate>(n => n != templateObj),
                It.IsAny<object>()))
                .Returns(new Dictionary<string, string>()
                    {
                        {"Message", ""}
                    });

            var service = new EmailNotificationService(logService.Object, emailService.Object,
                templateService.Object,
                "test@honeypot.com", "Support");

            // Act
            service.Notify(user.Object, templateObj, null);

            // Assert
            logService.Verify(l => l.CreateLog(It.IsAny<Domain.Log>()), Times.Once());
        }

        #endregion


    }
}
