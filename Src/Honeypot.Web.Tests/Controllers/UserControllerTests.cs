using System;
using System.Web.Mvc;
using Honeypot.Domain;
using Honeypot.Services;
using Honeypot.Web.Controllers;
using Honeypot.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Honeypot.Web.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        [TestMethod]
        public void User_SignUp_Returns_View()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = (ViewResult)controller.SignUp();

            // Assert
            Assert.AreEqual(result.ViewName, "SignUp");
        }

        [TestMethod]
        public void User_SignIn_Returns_View()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = (ViewResult)controller.SignIn();

            // Assert
            Assert.AreEqual(result.ViewName, "SignIn");
        }

        [TestMethod]
        public void User_SignOut_Redirects_To_Login()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = controller.SignOut();

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void User_SignOut_Displays_Message()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);

            // Act
            var result = (RedirectToRouteResult)controller.SignOut();

            // Assert
            Assert.AreNotEqual(String.Empty, result.RouteValues["message"]);
        }

        [TestMethod]
        public void User_EditProfile_Returns_EditProfile_View()
        {
            // Arrange
            var auth = new Mock<IAuth>();
            var userService = new Mock<IUserService>();
            var controller = new UserController(userService.Object);
            controller.CurrentUser = new User
            {
                Email = "testemail@honeypot.com"
            };

            // Act
            var result = (ViewResult)controller.EditProfile();

            // Assert
            Assert.AreEqual(result.ViewName, "EditProfile");
        }


    }
}
