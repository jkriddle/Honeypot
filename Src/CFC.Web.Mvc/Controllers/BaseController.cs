using System.Collections.Generic;
using System.IO;
using CFC.Domain;
using CFC.Services.UserService;
using CFC.Web.Mvc.MefFactory;


namespace CFC.Web.Mvc.Controllers
{
    using System.Web.Mvc;

    public class BaseController : Controller
    {
        private readonly IUserService _userService;
        public User CurrentUser { get; set; }
        public List<string> Plugins { get; set; }
        private readonly MefControllerFactory mefControllerFactory;
        public BaseController()
        {
            mefControllerFactory = new MefControllerFactory("Plugins\\");
            Plugins = mefControllerFactory.MyPlugins;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string username = requestContext.HttpContext.User.Identity.Name;
                CurrentUser = _userService.GetUserByEmail(username);
                ViewData["CurrentUser"] = CurrentUser;
            }
            else
                ViewData["CurrentUser"] = null;
        }

        /// <summary>
        /// Renders a specified view and returns it as a string
        /// </summary>
        /// <param name="viewName">View to render</param>
        /// <param name="model">View model data</param>
        /// <returns></returns>
        public string RenderView(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        /*// Executed up execution each controller action
        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            initializeBaseController(ctx);
        }

        protected void initializeBaseController(ActionExecutingContext ctx)
        {
            if (ctx.HttpContext.User.Identity.IsAuthenticated)
            {
                string username = ctx.HttpContext.User.Identity.Name;
                CurrentUser = _userService.GetUserByUsername(username);
                ViewData["CurrentUser"] = CurrentUser;
            }
            else
                ViewData["CurrentUser"] = null;
        }*/
    }
}
