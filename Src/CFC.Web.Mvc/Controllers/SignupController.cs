using CFC.Domain;
using CFC.Services.UserService;
using CFC.Web.Mvc.Attributes;
using CFC.Web.Mvc.Models;
using CFC.Web.Mvc.Models.Signup;
using CFC.Web.Mvc.Providers;
using CFC.Web.Mvc.Wrappers;

namespace CFC.Web.Mvc.Controllers
{
    using System.Web.Mvc;

    public class SignupController : BaseController
    {
        private readonly IAuth _authentication;
        private readonly IUserService _userService;

        public SignupController()
          
        {
          
        }




        /// <summary>
        /// Company signup completed.
        /// </summary>
        /// <returns></returns>
        public ActionResult Complete()
        {
            return View("Complete");
        }


        /// <summary>
        /// Ajax user signup submission
        /// </summary>
        /// <param name="inputViewModel">User entered parameters</param>
        /// <returns></returns>
        [HttpPost, NoCache]
        public ActionResult Index(SignupInputModel inputViewModel)
        {
            var user = new User
                           {
                                FirstName = inputViewModel.FirstName,
                                LastName = inputViewModel.LastName,
                                Role = Role.Role1,
                                Email = inputViewModel.Email,
                                State = inputViewModel.State,
                                CellPhone = inputViewModel.CellPhone
                            };

            // Setup password and salt
            _userService.GenerateUserPassword(user, inputViewModel.Password);

            var validationState = new ModelStateWrapper(ModelState);

            var vm = new JsonResponseModel();

            // If both the view model and the user object are valid
            if (_userService.ValidateUser(user, validationState))
            {
                _userService.CreateUser(user);
                _authentication.DoAuth(user.Email, false);
                vm.Success = true;
                return Json(vm, JsonRequestBehavior.AllowGet);
            }

            vm.Errors = validationState.Errors;

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

    }
}
