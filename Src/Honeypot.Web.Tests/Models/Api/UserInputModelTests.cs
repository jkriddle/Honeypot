﻿using System.IO;
using Honeypot.Services;
using Honeypot.Web.Areas.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Honeypot.Web.Tests.Models.Api
{
    [TestClass]
    public class UserInputModelTests
    {

        [TestMethod]
        public void Validate_Fails_Mismatched_Passwords()
        {
            // Arrange
            var model = new UpdateUserInputModel
                {
                    UserId = 1,
                    Email = "testupdate@honeypot.com",
                    Password = "goodpass",
                    ConfirmPassword = "badpass"
                };
            var validationDictionary = new ValidationDictionary();

            // Act

            var result = model.ValidateRequest(validationDictionary);

            // Assert
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Validate_Successful_With_Valid_Information()
        {
            // Arrange
            var model = new UpdateUserInputModel
            {
                UserId = 1,
                Email = "testupdate@honeypot.com",
                Password = "goodpass",
                ConfirmPassword = "goodpass"
            };
            var validationDictionary = new ValidationDictionary();

            // Act

            var result = model.ValidateRequest(validationDictionary);

            // Assert
            Assert.IsTrue(result);
        }

    }
}

