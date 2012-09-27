using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CFC.Domain;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace CFC.Tests.CFC.Services
{
    public class BaseServiceTests : DynamicObject
    {
        /// <summary>
        /// Gets the valid user.
        /// </summary>
        /// <returns></returns>
        public User GetValidUser()
        {
            return new User
                       {
                           AccessToken = "1234",
                           CellPhone = "4105555555",
                           Email = "test@mailinator.com",
                           FacebookId = 123,
                           FirstName = "John",
                           HashedPassword = Encoding.ASCII.GetBytes("123"),
                           LastName = "Doe",
                           ResetPasswordToken = "123",
                           Role = Role.Role1,
                           Salt = Encoding.ASCII.GetBytes("123")
                       };
        }


        /// <summary>
        /// Gets or sets the state of the model.
        /// </summary>
        /// <value>
        /// The state of the model.
        /// </value>
        public ModelStateDictionary ModelState
        {
            get { return ModelState = new ModelStateDictionary(); }
            set
            {
                // Method not used
            }
        }
    }
}
