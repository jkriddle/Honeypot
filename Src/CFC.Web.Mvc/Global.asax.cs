using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Web;
using CFC.Infrastructure;
using CFC.Web.Mvc.Bootstrap;
using CFC.Web.Mvc.MefFactory;
using Castle.Core.Logging;
using Rejuicer;

namespace CFC.Web.Mvc
{
    using System;
    using System.Reflection;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Castle.Windsor;

    // CFC.Web.Mvc.CastleWindsor
    using CastleWindsor;

    using CommonServiceLocator.WindsorAdapter;

    using Controllers;
    
    using Infrastructure.NHibernateMaps;

    using SharpArch.Domain.Events;

    using log4net.Config;
    
    using Microsoft.Practices.ServiceLocation;
    
    using SharpArch.NHibernate;
    using SharpArch.NHibernate.Web.Mvc;
    using SharpArch.Web.Mvc.Castle;
    using SharpArch.Web.Mvc.ModelBinder;
    

    /// <summary>
    /// Represents the MVC Application
    /// </summary>
    /// <remarks>
    /// For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </remarks>
    public class MvcApplication : System.Web.HttpApplication
    {
        private WebSessionStorage _webSessionStorage;
    
     

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

            // Facebook API
            HttpContext.Current.Response.AddHeader("p3p", "CP=\"CAO PSA OUR\"");

            // Cross domain requests
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            // Log the exception.
            //ILogger logger = Container.Resolve<ILogger>();
            //logger.Error(exception);

            return;

            Response.Clear();

            var httpException = exception as HttpException;

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");

            if (httpException == null)
            {
                routeData.Values.Add("action", "Index");
            }
            else //It's an Http Exception, Let's handle it.
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // Page not found.
                        routeData.Values.Add("action", "HttpStatus404");
                        break;
                    case 500:
                        // Server error.
                        routeData.Values.Add("action", "HttpStatus500");
                        break;

                    // Here you can handle Views to other error codes.
                    // I choose a General error template  
                    default:
                        routeData.Values.Add("action", "General");
                        break;
                }
            }

            // Pass exception details to the target error View.
            routeData.Values.Add("error", exception);

            // Clear the error on server.
            Server.ClearError();

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData.
            IController errorController = new CommonController();
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        }


        protected void Application_Start()
        {
            XmlConfigurator.Configure();

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize(); 

            ViewEngines.Engines.Clear();

            ViewEngines.Engines.Add(new RazorViewEngine());
            
            ModelBinders.Binders.DefaultBinder = new SharpModelBinder();

            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            this.InitializeServiceLocator();


            //MEF Plugin Factory
          

            ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory(
                   Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\")));

            //MEF Plugin Factory


            AreaRegistration.RegisterAllAreas();
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);
            

            AutoMapperBootstrap.Configure();

            OnRequest.ForJs("~/App.js")
                        .Combine
                        //.Compact   -- change above line to compact in productino
                        .File("~/Scripts/jquery-1.7.2.min.js")
                        .File("~/Scripts/jquery.signalR-0.5.2.min.js")
                        .FilesIn("~/Content/js/")
                        .Matching("*.js")
                        .File("~/Scripts/jquery-ui-1.8.20.min.js")
                        .FilesIn("~/Content/js/app/")
                        .Matching("*.js")
                        .FilesIn("~/Content/js/formwizard/")
                        .Matching("*.js")
                        .FilesIn("~/Content/js/app/models")
                        .Matching("*.js")
                        .FilesIn("~/Content/js/app/plugins/")
                        .Matching("*.js")
                        .FilesIn("~/Content/js/app/viewmodels/")
                        .Matching("*.js")
                        .Configure();

            OnRequest.ForCss("~/App.css")
                        .Combine
                        //.Compact   -- change above line to compact in productino
                        .FilesIn("~/Content/css/")
                        .Matching("*.css")
                        .Configure();
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from
        /// WindsorController to the container.  Also associate the Controller
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeServiceLocator() 
        {
            var container = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));

            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.AddComponentsTo(container);

            var windsorServiceLocator = new WindsorServiceLocator(container);
            DomainEvents.ServiceLocator = windsorServiceLocator;
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }

        private void InitialiseNHibernateSessions()
        {
            NHibernateSession.ConfigurationCache = null; // new NHibernateConfigurationFileCache();

            NHibernateSession.Init(
                this._webSessionStorage,
                new[] { Server.MapPath("~/bin/CFC.Infrastructure.dll") },
                new AutoPersistenceModelGenerator().Generate(),
                Server.MapPath("~/NHibernate.config"));
        }
    }
}