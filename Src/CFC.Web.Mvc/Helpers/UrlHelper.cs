using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CFC.Web.Mvc.Helpers
{
    public static class DomainHelper
    {

        public static string AbsoluteUrl(string path, bool forceHttps = false)
        {
            string serverUrl = VirtualPathUtility.ToAbsolute(path);

            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            string newUrl = serverUrl;
            var domain = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
            Uri originalUri = HttpContext.Current.Request.Url;

            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                "://" + domain + newUrl;
            return newUrl;
        } 

    }
}