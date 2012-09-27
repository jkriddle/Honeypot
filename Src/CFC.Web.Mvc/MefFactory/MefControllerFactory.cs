using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace CFC.Web.Mvc.MefFactory
{
    public class MefControllerFactory : IControllerFactory
    {
        private string _pluginPath;
        private DirectoryCatalog _catalog; 
        private DefaultControllerFactory _defaultControllerFactory;
        
        private  readonly CompositionContainer _container;

   
        public MefControllerFactory(string pluginPath)
        {

            this._pluginPath = pluginPath;
            this._catalog = new DirectoryCatalog(pluginPath);
            this._container = new CompositionContainer(_catalog);
            this._defaultControllerFactory = new DefaultControllerFactory();
        }
        #region IControllerFactory Members

        public List<string> MyPlugins
        {
            get
            {
                var controllers = _container.GetExports<IController, IDictionary<String, object>>();
                return (from controller in controllers 
                        where controller
                            .Metadata
                            .ContainsKey("Name") 
                        select controller
                            .Metadata["Name"]
                            .ToString())
                            .ToList();
            }
        }

        public IController CreateController(RequestContext requestContext, string controllerName)
        {
         
            
            IController controller = null;

            if (controllerName != null)
            {
                //var exports = _container.GetExports<IController, INameMetadata>();
                //var exports1 = _container.GetExports<IController>();
                var export = _container.GetExports<IController, IDictionary<String,object>>()
                    .FirstOrDefault(e => e.Metadata.ContainsKey("Name")
                    && e.Metadata["Name"].ToString() == controllerName);
                if (export != null)
                {
                    
                    controller = export.Value;
                }
            }

            if (controller == null)
            {
                controller =  this._defaultControllerFactory.CreateController(requestContext, controllerName);
                
            }

            return controller;
             
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }
        
   


        public  void ReleaseController(IController controller)
        {

            var disposable = controller as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion
    }
}