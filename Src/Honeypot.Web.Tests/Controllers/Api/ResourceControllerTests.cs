using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using Honeypot.Services;
using Honeypot.Web.Areas.Api.Controllers;
using Honeypot.Web.Areas.Api.Models;
using Honeypot.Web.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Honeypot.Web.Tests.Controllers.Api
{
    [TestClass]
    public class ResourceControllerTests
    {
        #region

        private Resource CreateResource(int id)
        {
            var resource = new Mock<Resource>();
            resource.SetupProperty(l => l.Id, id);
            resource.SetupProperty(l => l.Name, "test");
            resource.SetupProperty(l => l.Value, "penguin");
            resource.SetupProperty(l => l.Type, ResourceType.Html);
            resource.SetupProperty(l => l.Category, ResourceCategory.Content);
            return resource.Object;
        }

        private ResourceController CreateResourceController(IResourceService resourceService)
        {
            var userService = new Mock<IUserService>();
            var logService = new Mock<ILogService>();
            var mapper = new Mock<IMapperService>();
            var controller = new ResourceController(userService.Object, resourceService, logService.Object, mapper.Object);
            var ctx = new Mock<HttpControllerContext>();
            controller.ControllerContext = ctx.Object;
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://localhost", ""),
                new HttpResponse(new StringWriter())
            );
            return controller;
        }

        #endregion

        #region Get

        [TestMethod]
        public void GetOne_Returns_Resource()
        {
            // Arrange
            var resource = CreateResource(1);
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(l => l.GetResourceById(It.IsAny<int>())).Returns(resource);
            var controller = CreateResourceController(resourceService.Object);

            // Act
            var result = controller.GetOne(1);

            // Assert
            Assert.AreEqual(resource.Id, result.ResourceId);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void GetOne_Fails_When_Not_Found()
        {
            // Arrange
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(l => l.GetResourceById(It.IsAny<int>())).Returns((Resource)null);
            var controller = CreateResourceController(resourceService.Object);
            var user = new Mock<User>();
            user.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(false);

            // Act
            var result = controller.GetOne(1);

            // Asserted by attribute
        }

        [TestMethod]
        public void Get_Returns_ResourceListViewModel()
        {
            // Arrange
            var resource = CreateResource(1);
            var resourceService = new Mock<IResourceService>();
            var resourceList = new PagedList<Resource>(new List<Resource>
                {
                    resource
                }, 1, 10);
            resourceService.Setup(l => l.GetResources(It.IsAny<ResourceFilter>(),
                It.IsAny<int>(), It.IsAny<int>())).Returns(resourceList);
            var controller = CreateResourceController(resourceService.Object);
            var user = new Mock<User>();
            user.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(false);

            // Act
            var result = controller.Get(new ResourceListInputModel());

            // Assert
            Assert.IsInstanceOfType(result, typeof (ResourceListViewModel));
        }

        #endregion

        #region Update


        [TestMethod]
        public void Update_Successful_When_Valid()
        {
            // Arrange
            var resource = CreateResource(1);
            var logService = new Mock<ILogService>();
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(l => l.GetResourceById(It.IsAny<int>())).Returns(resource);
            resourceService.Setup(l => l.ValidateResource(It.IsAny<Resource>(), 
                It.IsAny<IValidationDictionary>())).Returns(true);
            var controller = CreateResourceController(resourceService.Object);
            var user = new Mock<User>();
            user.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(true);
            controller.CurrentUser = user.Object;

            // Act
            var result = controller.Update(new UpdateResourceInputModel()
            {
                ResourceId = 1,
                Value = "test"
            });

            // Assert
            resourceService.Verify(r => r.UpdateResource(resource), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void Update_Fails_When_Not_Found()
        {
            // Arrange
            var resource = CreateResource(1);
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(l => l.GetResourceById(It.IsAny<int>())).Returns((Resource)null);
            var controller = CreateResourceController(resourceService.Object);
            var user = new Mock<User>();
            user.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(false);
            controller.CurrentUser = user.Object;

            // Act
            var result = controller.Update(new UpdateResourceInputModel()
                {
                    ResourceId = 999,
                    Value = "test"
                });

            // Asserted by attribute
        }

        [TestMethod]
        [ExpectedException(typeof(HttpException))]
        public void Update_Fails_Without_Permissions()
        {
            // Arrange
            var resource = CreateResource(1);
            var resourceService = new Mock<IResourceService>();
            resourceService.Setup(l => l.GetResourceById(It.IsAny<int>())).Returns(resource);
            var controller = CreateResourceController(resourceService.Object);
            var user = new Mock<User>();
            user.Setup(u => u.HasPermission(It.IsAny<string>())).Returns(false);
            controller.CurrentUser = user.Object;

            // Act
            var result = controller.Update(new UpdateResourceInputModel()
            {
                ResourceId = 1,
                Value = "test"
            });

            // Asserted by attribute
        }

        #endregion

    }
}

