﻿using System.Web;
using System.Web.Http;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Honeypot.Domain;
using Honeypot.Services;
using Honeypot.Web.Bootstrap;
using Honeypot.Web.Controllers;
using Honeypot.Infrastructure.Mapping;
using SharpArch.Domain.Events;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.Castle;
using SharpArch.Web.Mvc.ModelBinder;

namespace Honeypot.Web
{
    
    /// <summary>
    /// Represents the MVC Application
    /// </summary>
    /// <remarks>
    /// For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </remarks>
    public class MvcApplication : HttpApplication
    {
        #region Fields

        private WebSessionStorage _webSessionStorage;

        #endregion

        #region HttpApplication Events/Overrides

        /// <summary>
        /// Due to issues on IIS7, the NHibernate initialization must occur in Init().
        /// But Init() may be invoked more than once; accordingly, we introduce a thread-safe
        /// mechanism to ensure it's only initialized once.
        /// See http://msdn.microsoft.com/en-us/magazine/cc188793.aspx for explanation details.
        /// </summary>
        public override void Init()
        {
            base.Init();
            this._webSessionStorage = new WebSessionStorage(this);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            NHibernateInitializer.Instance().InitializeNHibernateOnce(this.InitialiseNHibernateSessions);
        }

        protected void Application_Start()
        {
            XmlConfigurator.Configure();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            ModelBinders.Binders.DefaultBinder = new SharpModelBinder();
            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            InitializeServiceLocator();

            AreaRegistration.RegisterAllAreas();
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            AutoMapperServiceBootstrap.Init();

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            try
            {
                // Attempt to log the error
                var logService = ServiceLocator.Current.GetInstance<ILogService>();
                logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = exception.Message + " at " + HttpContext.Current.Request.Url.ToString(),
                    Details = exception.StackTrace,
                    Level = LogLevel.Error,
                });
            }
            catch (Exception)
            {

            }
        }

        #endregion

        #region Private Helper Methods

        private void InitialiseNHibernateSessions()
        {
            NHibernateSession.ConfigurationCache = null; // new NHibernateConfigurationFileCache();

            NHibernateSession.Init(
                this._webSessionStorage,
                new[] { Server.MapPath("~/bin/Honeypot.Infrastructure.dll") },
                new AutoPersistenceModelGenerator().Generate(),
                Server.MapPath("~/NHibernate.config"));
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from
        /// WindsorController to the container.  Also associate the Controller
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeServiceLocator()
        {
            var container = new WindsorContainer();

            // Initialize MVC application
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            // Initialize WebApi
            container.Register(AllTypes.FromAssembly(typeof(HomeController).Assembly)
                                        .BasedOn<ApiController>()
                                        .If(r => r.Name.EndsWith("Controller"))
                                        .LifestyleTransient());

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorCompositionRoot(container));
            
            GlobalConfiguration.Configuration.DependencyResolver = new WindsorDependencyResolver(container);

            // Setup service locator
            var windsorServiceLocator = new WindsorServiceLocator(container);
            DomainEvents.ServiceLocator = windsorServiceLocator;
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }

        #endregion

    }
}