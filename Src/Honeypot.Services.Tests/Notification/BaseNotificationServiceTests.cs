using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Honeypot.Services.Tests
{
    [TestClass]
    public class BaseNotificationServiceTests
    {

        #region Authenticate

        [TestMethod]
        public void ParseTemplate_Replaces_Tokens()
        {
            // Arrange
            var logService = new Mock<ILogService>();
            var service = new Mock<BaseNotificationService>(logService.Object);
            var template = new Mock<INotificationTemplate>();
            template.Setup(r => r.Read()).Returns(
                new XElement("Sample",
                    new XElement("Subject", "My favorite food is"),
                    new XElement("Body", "a {{Favorite}}")));

            // Act
            var result = service.Object.ParseTemplate("Sample", template.Object, new
            {
                Favorite = "Pizza"
            });

            // Assert
            Assert.AreEqual(result["Body"], "a Pizza");
        }

        [TestMethod]
        public void ParseTemplate_Does_Not_Replaces_Matching_NonTokens()
        {
            // Arrange
            var logService = new Mock<ILogService>();
            var service = new Mock<BaseNotificationService>(logService.Object);
            var template = new Mock<INotificationTemplate>();
            template.Setup(r => r.Read()).Returns(
                new XElement("Email",
                    new XElement("Subject", "Favorite"),
                    new XElement("Body", "{{Favorite}}")));

            // Act
            var result = service.Object.ParseTemplate("Sample", template.Object, new
                {
                    Favorite = "Pizza"
                });

            // Assert
            Assert.AreEqual(result["Subject"], "Favorite");
        }

        #endregion


    }
}
