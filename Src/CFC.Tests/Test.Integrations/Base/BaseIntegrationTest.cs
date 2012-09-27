using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CFC.Domain;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpArch.NHibernate;

namespace CFC.Tests.CFC.Integrations
{
    public class BaseIntegrationTest : DynamicObject
    {
        #region Setup & TearDown
        /// <summary>
        /// Set Configuration, Mapping & init ServiceLocator
        /// These tests are SLOW...  
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            new InitSessionConnection();
            ServiceLocatorInitializer.Init();
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize(); 
        }

      

        /// <summary>
        /// Tears down.
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            NHibernateSession.CloseAllSessions();
            NHibernateSession.Reset();
        }
        #endregion Setup & TearDown

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
