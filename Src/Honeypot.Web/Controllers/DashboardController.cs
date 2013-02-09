using System.Web.Mvc;
using Honeypot.Web.Attributes;

namespace Honeypot.Web.Controllers
{
    public class DashboardController : Controller
    {
        [RequiresAuthentication]
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}
