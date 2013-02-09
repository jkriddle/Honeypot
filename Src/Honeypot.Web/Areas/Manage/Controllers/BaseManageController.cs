using System.Web.Mvc;
using Honeypot.Domain;
using Honeypot.Web.Attributes;

namespace Honeypot.Web.Areas.Manage.Controllers
{
    [RequiresAuthentication, RequiresRole(Role.Administrator)]
    public class BaseManageController : Controller
    {
    }
}