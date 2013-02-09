using System.Web.Caching;
using Honeypot.Domain;
using Honeypot.Services;
using Microsoft.Practices.ServiceLocation;

namespace Honeypot.Web.Helpers
{
    public static class ResourceHelper
    {
        #region Fields
        
        #endregion

        #region Properties

        private static IResourceService ResourceService { 
            get
            {
                return ServiceLocator.Current.GetInstance<IResourceService>();
            }
        }

        #endregion

        #region Public Methods

        public static string GetString(string name)
        {
            var resource = ResourceService.GetResourceByName(name);
            if (resource == null) return "[Resource not found: " + name + "]";
            return resource.Value;
        }

        #endregion

        #region Private Methods

        #endregion

    }
}