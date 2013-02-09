using System.Web.Mvc;

namespace Honeypot.Web.Areas.Manage.Controllers
{
    public class DashboardController : BaseManageController
    {
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}
