using System.ComponentModel.Composition;
using CFC.Services.UserService;
using CFC.Web.Mvc.MefFactory;
using CFC.Web.Mvc.Models.Navigation.View;
using CFC.Web.Mvc.Providers;

namespace CFC.Web.Mvc.Controllers
{
    using System.Web.Mvc;
    [Export(typeof(IController))]
    [ExportMetadata("controllerName", "Home")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HomeController : BaseController
    {
       
        public HomeController()
            
        {
          
        }

        
        public ActionResult Index()
        {
            var pluginViewModel = new PluginViewModel {Plugins = Plugins};
            return View(pluginViewModel);
        }

        [HttpPost]
        public ActionResult Index(string username, string password, bool rememberMe = false)
        {
            return View();
        }

    }
}
