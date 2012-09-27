using System.Web.Mvc;
using CFC.Services.UserService;
using CFC.Web.Mvc.Models.Utility;

namespace CFC.Web.Mvc.Controllers
{
    public class UtilityController : BaseController
    {
        public UtilityController() 
        {
        }

        /// <summary>
        /// Display utility panel
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var vm = new UtilityViewModel(CurrentUser, new Providers.SessionProvider(ControllerContext));
            return View("Index", vm);
        }

    }
}
