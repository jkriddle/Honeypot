using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CFC.Web.Mvc.Controllers;

namespace CFC.Web.Mvc.Attributes
{
    /// <summary>
    /// Verifies that user is logged into system.
    /// </summary>
    public class RequiresAuthenticationAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //redirect if not authenticated
            var controller = filterContext.Controller as BaseController;

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated && (controller == null || controller.CurrentUser == null))
            {
                //use the current url for the redirect
                string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;

                //send them off to the login page
                string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                string loginUrl = FormsAuthentication.LoginUrl + redirectUrl;
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}