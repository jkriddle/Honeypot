using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CFC.Web.Mvc.Controllers;
using CFC.Web.Mvc.Models;
using CFC.Services.UserService;
using Microsoft.Practices.ServiceLocation;
using CFC.Domain;

namespace CFC.Web.Mvc.Attributes
{
    /// <summary>
    /// Check the parameters being sent in the controller's view model, and ensure
    /// the auth token is valid.
    /// </summary>
    public class ApiAuth : RequiresAuthenticationAttribute
    {
        private const string _authTokenParamKey = "AuthToken";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string authTokenParam = filterContext.HttpContext.Request.Params[_authTokenParamKey];

            if (authTokenParam != null)
            {
                // Verify user
                var userService = ServiceLocator.Current.GetInstance<IUserService>();

                User user = userService.Authenticate(authTokenParam);
                if (user == null)
                {
                    throw new Exception("Unable to validate your device credentials.");
                }

                var controller = filterContext.Controller as BaseController;
                controller.CurrentUser = user;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}