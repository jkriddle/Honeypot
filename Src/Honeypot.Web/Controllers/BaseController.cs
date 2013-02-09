using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Honeypot.Domain;
using Honeypot.Services;

namespace Honeypot.Web.Controllers
{
    public class BaseController : Controller
    {
        #region Fields

        private readonly IUserService _userService;

        #endregion

        #region Constructor

        public BaseController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region Public Properties

        public User CurrentUser { get; set; }

        #endregion

        #region Overrides

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string username = requestContext.HttpContext.User.Identity.Name;
                CurrentUser = _userService.GetUserByEmail(username);
            }
        }

        #endregion

    }
}