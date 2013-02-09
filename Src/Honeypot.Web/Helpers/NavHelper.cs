using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Honeypot.Web.Helpers
{
    public static class NavHelper
    {
        /// <summary>
        /// If specified path is the currently viewed URL path, return an class
        /// name to use to mark the menu nav item as active.
        /// </summary>
        /// <param name="path">Path to compare current Url to</param>
        /// <param name="additionalClasses">Additional CSS classes to append if currently active</param>
        /// <returns></returns>
        public static string ActiveClass(string path, string additionalClasses = "")
        {
            return path == HttpContext.Current.Request.Url.AbsolutePath ? "active " + additionalClasses : "";
        }
    }
}