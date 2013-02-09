using System.Web.Mvc;

namespace Honeypot.Web.Areas.Manage.Controllers
{
    public class UserController : BaseManageController
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Edit()
        {
            return View("Edit");
        }

        public ActionResult Details()
        {
            return View("Details");
        }
    }
}
