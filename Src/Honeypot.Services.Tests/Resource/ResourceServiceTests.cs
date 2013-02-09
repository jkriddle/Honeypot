using System;
using System.Collections.Generic;
using System.Linq;
using Honeypot.Domain;
using Honeypot.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Honeypot.Services.Tests
{
    [TestClass]
    public class ResourceServiceTests
    {
        #region Helper Methods

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

        private ResourceService CreateResourceService(IResourceRepository resourceRepository)
        {
            return new ResourceService(resourceRepository);
        }

        #endregion

        #region Get

        [TestMethod]
        public void GetResourceById_Retrieves_Resource_With_Matching_Id()
        {
            // Arrange
            var resource = CreateResource(1);
            var resourceRepository = new Mock<IResourceRepository>();
            resourceRepository.Setup(r => r.FindOne(It.IsAny<int>())).Returns(resource);
            var resourceService = CreateResourceService(resourceRepository.Object);

            // Act
            var result = resourceService.GetResourceById(1);

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetResourceByName_Retrieves_Resource_With_Matching_Name()
        {
            // Arrange
            var resource = CreateResource(1);
            var resourceList = new List<Resource>()
                {
                    resource
                };
            var resourceRepository = new Mock<IResourceRepository>();
            resourceRepository.Setup(r => r.FindAll()).Returns(resourceList.AsQueryable());
            var resourceService = CreateResourceService(resourceRepository.Object);

            // Act
            var result = resourceService.GetResourceByName("test");

            // Assert
            Assert.AreEqual(1, result.Id);
        }

        #endregion

        #region Update

        [TestMethod]
        public void Update_Resource_Successful_If_Not_Readonly()
        {
            // Arrange
            var resource = new Mock<Resource>();
            resource.SetupProperty(r => r.IsReadOnly, false);
            resource.SetupProperty(r => r.Name, "test");
            var resourceRepository = new Mock<IResourceRepository>();
            resourceRepository.Setup(r => r.Save(It.IsAny<Resource>())).Verifiable();
            var resourceService = CreateResourceService(resourceRepository.Object);

            // Act
            resourceService.UpdateResource(resource.Object);

            // Asserted by exception attribute
            resourceRepository.Verify(r => r.Save(resource.Object), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Update_Resource_Fails_If_Readonly()
        {
            // Arrange
            var resource = new Mock<Resource>();
            resource.SetupProperty(r => r.IsReadOnly, true);
            resource.SetupProperty(r => r.Name, "test");
            var resourceRepository = new Mock<IResourceRepository>();
            var resourceService = CreateResourceService(resourceRepository.Object);

            // Act
            resourceService.UpdateResource(resource.Object);

            // Asserted by exception attribute
        }

        #endregion

        #region Validation

        [TestMethod]
        public void Validation_Success_With_Valid_Resource()
        {
            // Arrange
            var resource = CreateResource(1);
            var resourceRepository = new Mock<IResourceRepository>();
            var resourceService = CreateResourceService(resourceRepository.Object);
            var validationDictionary = new ValidationDictionary();

            // Act
            resourceService.ValidateResource(resource, validationDictionary);

            // Assert
            Assert.IsTrue(validationDictionary.IsValid);
        }

        [TestMethod]
        public void Validation_Fails_Missing_Name()
        {
            // Arrange
            var resource = new Mock<Resource>();
            resource.SetupProperty(l => l.Id, 1);
            resource.SetupProperty(l => l.Value, "penguin");
            resource.SetupProperty(l => l.Type, ResourceType.Html);
            resource.SetupProperty(l => l.Category, ResourceCategory.Content);
            var resourceRepository = new Mock<IResourceRepository>();
            var resourceService = CreateResourceService(resourceRepository.Object);
            var validationDictionary = new ValidationDictionary();

            // Act
            resourceService.ValidateResource(resource.Object, validationDictionary);

            // Assert
            Assert.IsFalse(validationDictionary.IsValid);
        }

        #endregion

    }
}
