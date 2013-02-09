using System;
using System.Web.Mvc;
using Honeypot.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Honeypot.Web.Tests.Controllers
{
    [TestClass]
    public class DashboardControllerTests
    {
        [TestMethod]
        public void Dashboard_Index_Returns_View()
        {
            // Arrange
            var controller = new DashboardController();

            // Act
            var result = (ViewResult)controller.Index();

            // Assert
            Assert.AreEqual(result.ViewName, "Index");
        }
    }
}
