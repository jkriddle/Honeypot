using System.Net;

namespace CFC.Web.Mvc.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Site-wide actions for exceptional cases.
    /// </summary>
    public class CommonController : Controller
    {
        public ActionResult General()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View("General");
        }

        public ActionResult HttpStatus404()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("HttpStatus404");
        }

        public ActionResult HttpStatus500()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Just using general for now
            return View("General");
        }

    }
}
