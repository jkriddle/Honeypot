using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Plugin.Demo.Controllers
{
    [Export(typeof(IController))]
    [ExportMetadata("Name", "Demo")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DemoController : Controller
    {
        //
        // GET: /Demo/

        public ActionResult Index()
        {
            return View("~/Plugins/DemoViews/Demo/Index.cshtml");
        }

    }
}
