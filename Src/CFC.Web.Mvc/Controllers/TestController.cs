using System.Web.Mvc;
using CFC.Services.UserService;
namespace CFC.Web.Mvc.Controllers
{
    public class TestController : BaseController
    {

        private readonly IUserService _userService;



        public TestController()
           
        {
          
        }

        public ActionResult Index()
        {
            return View("Index");
        }

    }
}
