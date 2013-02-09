using System.Web.Mvc;
using Honeypot.Web.Areas.Manage.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Honeypot.Web.Tests.Controllers.Manage
{
    [TestClass]
    public class LogControllerTests
    {
        [TestMethod]
        public void Log_Index_Returns_View()
        {
            // Arrange
            var controller = new LogController();

            // Act
            var result = (ViewResult)controller.Index();

            // Assert
            Assert.AreEqual(result.ViewName, "Index");
        }

        [TestMethod]
        public void Log_View_Returns_View()
        {
            // Arrange
            var controller = new LogController();

            // Act
            var result = (ViewResult)controller.Details();

            // Assert
            Assert.AreEqual(result.ViewName, "Details");
        }
    }
}
