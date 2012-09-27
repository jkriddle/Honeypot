using CFC.Services.UserService;
using CFC.Web.Mvc.Providers;

namespace CFC.Web.Mvc.Controllers
{
    using System.Web.Mvc;

    public class LogoutController : BaseController
    {
        private readonly IAuth _authentication;
        public LogoutController()
           
        {
        
        }

        /// <summary>
        /// Log a user out of the system
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            _authentication.SignOut();
            return RedirectToAction("Index", "Home", null);
        }

    }
}
