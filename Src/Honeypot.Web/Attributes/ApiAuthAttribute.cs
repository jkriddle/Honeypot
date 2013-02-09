using System;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Security;
using Honeypot.Domain;
using Honeypot.Services;
using Honeypot.Web.Areas.Api.Controllers;
using Microsoft.Practices.ServiceLocation;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Honeypot.Web.Attributes
{
    /// <summary>
    /// Verifies that user is logged into system using the specified
    /// authorizatino token from the request headers
    /// </summary>
    public class ApiAuthAttribute : ActionFilterAttribute
    {
        private const string _authTokenParamKey = "AuthToken";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string authTokenParam = HttpContext.Current.Request.Headers[_authTokenParamKey];

            if (authTokenParam != null)
            {
                // Verify user
                var userService = ServiceLocator.Current.GetInstance<IUserService>();

                User user = userService.Authenticate(authTokenParam);
                if (user == null)
                {
                    throw new HttpException(401, "Unable to validate your device credentials.");
                }

                var controller = actionContext.ControllerContext.Controller as BaseController;
                controller.CurrentUser = user;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}