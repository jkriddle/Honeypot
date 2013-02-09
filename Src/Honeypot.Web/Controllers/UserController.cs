using System;
using System.Web.Mvc;
using Honeypot.Services;
using Honeypot.Web.Attributes;
using Honeypot.Web.Models;
using Honeypot.Web.Providers;

namespace Honeypot.Web.Controllers
{
    public class UserController : BaseController
    {

        #region Constructor

        public UserController(IUserService userService) 
            : base(userService)
        {
        }

        #endregion

        #region Actions

        [RequiresAuthentication]
        public ActionResult EditProfile()
        {
            return View("EditProfile");
        }

        public ActionResult SignIn(string message = "")
        {
            if (!String.IsNullOrWhiteSpace(message))
            {
                ViewBag.Message = message;
            }
            return View("SignIn");
        }

        public ActionResult SignUp()
        {
            return View("SignUp");
        }

        [NoCache]
        public ActionResult SignOut()
        {
            return RedirectToAction("SignIn", new { message = "You have been signed out." });
        }

        public ActionResult ResetPassword()
        {
            return View("ResetPassword");
        }


        #endregion

    }
}
