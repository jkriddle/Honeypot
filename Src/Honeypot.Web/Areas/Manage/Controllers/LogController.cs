using System.Web.Mvc;

namespace Honeypot.Web.Areas.Manage.Controllers
{
    public class LogController : BaseManageController
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Details()
        {
            return View("Details");
        }
    }
}
