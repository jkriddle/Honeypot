using System.Configuration;
using CFC.Domain;

using CFC.Services.UserService;
using CFC.Web.Mvc.Attributes;
using CFC.Web.Mvc.Helpers;
using CFC.Web.Mvc.Models;
using CFC.Web.Mvc.Models.Login;
using CFC.Web.Mvc.Providers;
using CFC.Web.Mvc.Resources;
using CFC.Web.Mvc.Wrappers;
using MEF.FacebookPlugin;


namespace CFC.Web.Mvc.Controllers
{
    using System.Web.Mvc;

    public class LoginController : BaseController
    {
        private readonly IAuth _authentication;
        private readonly IUserService _userService;
        private readonly IFacebookService _facebookService;

        public LoginController()
        
        {
       
        }

        private string RedirectUrl
        {
            get
            {
                return DomainHelper.AbsoluteUrl(Url.Action("OAuth", "Login"));
            }
        }

        /// <summary>
        /// Redirects user to Facebook's OAuth API
        /// </summary>
        /// <returns></returns>
        public ActionResult Facebook()
        {
            return
                Redirect(
                    "https://www.facebook.com/dialog/oauth?client_id=" + ConfigurationManager.AppSettings["FacebookId"]
                    + "&redirect_uri=" + RedirectUrl
                    + "&scope=email&state=cfcauth");
        }

        /// <summary>
        /// User is returning back from Facebook authorization screen.
        /// </summary>
        /// <param name="inputModel">Facebook authorization information</param>
        /// <returns></returns>
        public ActionResult OAuth(FacebookAuthInputModel inputModel)
        {
            if (inputModel.IsAuthorized)
            {
                // Log user into system if already registered.
                string token = _facebookService.GetFacebookAccessToken(inputModel.Code, RedirectUrl);
                
                UserProfile profile = _facebookService.GetFacebookProfile(token);
                if (_userService.AuthenticateFacebookUser(profile))
                {
                    _authentication.DoAuth(profile.Email, false);
                    return RedirectToAction("Index", "Home", null);
                }
                // User is not registered. Create an account for them.
                _userService.RegisterFacebookUser(profile);
                _authentication.DoAuth(profile.Email, false);
                return RedirectToAction("Index", "Home", null);
            }

            return View("NotAuthorized", inputModel);
        }
        
        /// <summary>
        /// Ajax user login function
        /// </summary>
        /// <param name="inputViewModel">User input model</param>
        /// <returns></returns>
        [HttpPost, NoCache]
        public ActionResult Index(LoginInputModel inputViewModel)
        {
            var vm = new LoginResponseModel();
            User user = _userService.Authenticate(inputViewModel.Email, inputViewModel.Password, inputViewModel.HardwareId);
            if (user != null)
            {
                _authentication.DoAuth(inputViewModel.Email, false);
                vm.Success = true;
                vm.Role = user.Role.ToString();
                // Web-based API (for testing)
                vm.AuthToken = user.AuthToken;
            }
            else
            {
                vm.Message = AppResources.InvalidCredentials;
                vm.Success = false;
            }
            
            return Json(vm, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Ajax method for initiating forgotten password
        /// </summary>
        /// <returns></returns>
        [HttpPost, NoCache]
        public ActionResult Forgot(ForgotPasswordInputModel inputModel)
        {
            var vm = new JsonResponseModel();
            
            var validationState = new ModelStateWrapper(ModelState);

            if (ModelState.IsValid)
            {
                // Generate reset password link
                if (_userService.GenerateForgotPasswordLink(Url.Action("Reset", "Login", null, "http"), inputModel.Email))
                {
                    vm.Success = true;
                    vm.Message = AppResources.PasswordResetLinkSent;
                } else
                {
                    vm.Success = false;
                    validationState.AddError("Email", AppResources.AccountNotFound);
                }
            }

            vm.Errors = validationState.Errors;

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Password reset page
        /// </summary>
        /// <returns></returns>
        public ActionResult Reset(string token)
        {
            return View("Reset");
        }

        /// <summary>
        /// Password reset page
        /// </summary>
        /// <returns></returns>
        [HttpPost, NoCache]
        public ActionResult Reset(ResetPasswordInputModel inputModel)
        {
            var vm = new ResetPasswordResponseModel();
            var validationState = new ModelStateWrapper(ModelState);

            if (validationState.IsValid)
            {
                // Must retrieve user first (token is cleared after resetting).
                User user = _userService.GetUserByResetToken(inputModel.Token);
                if (_userService.ResetPassword(inputModel.Token, inputModel.Password))
                {
                    _authentication.DoAuth(user.Email, false);
                    vm.Message = AppResources.ResetSuccessful;
                    vm.Success = true;
                } else
                {
                    vm.Message = AppResources.ResetKeyNotFound;
                    vm.Success = false;
                }
            }

            vm.Errors = validationState.Errors;
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

    }
}
