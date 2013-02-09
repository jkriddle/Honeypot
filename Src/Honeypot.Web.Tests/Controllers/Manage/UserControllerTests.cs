using System.Web.Mvc;
using Honeypot.Web.Areas.Manage.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Honeypot.Web.Tests.Controllers.Manage
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void User_Index_Returns_View()
        {
            // Arrange
            var controller = new UserController();

            // Act
            var result = (ViewResult)controller.Index();

            // Assert
            Assert.AreEqual(result.ViewName, "Index");
        }


        [TestMethod]
        public void User_Edit_Returns_View()
        {
            // Arrange
            var controller = new UserController();

            // Act
            var result = (ViewResult)controller.Edit();

            // Assert
            Assert.AreEqual(result.ViewName, "Edit");
        }


        [TestMethod]
        public void User_View_Returns_View()
        {
            // Arrange
            var controller = new UserController();

            // Act
            var result = (ViewResult)controller.Details();

            // Assert
            Assert.AreEqual(result.ViewName, "Details");
        }
    }
}
