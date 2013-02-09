using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using Honeypot.Services;
using Honeypot.Web.Areas.Api.Models;
using Honeypot.Web.Attributes;
using Honeypot.Web.Providers;

namespace Honeypot.Web.Areas.Api.Controllers
{
    [NoCache]
    public class UserController : BaseController
    {
        #region Fields

        private readonly IAuth _auth;
        private readonly IMapperService _mapper;

        #endregion

        #region Constructor

        public UserController(IAuth auth, 
            IUserService userService, 
            ILogService logService,
            IMapperService mapper) : base(userService, logService)
        {
            _auth = auth;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve a single user by ID
        /// </summary>
        /// <param name="id">User's ID</param>
        /// <returns>User data</returns>
        [HttpGet, ApiAuth]
        public UserViewModel GetOne(int? id)
        {
            var user = id.HasValue ? UserService.GetUserById(id.Value) : CurrentUser;
            if (user == null)
            {
                throw new HttpException(404, "User not found.");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditUsers)
                && user.Id != CurrentUser.Id)
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            return new UserViewModel(user);
        }

        /// <summary>
        /// Retrieve a list of users based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of users</returns>
        [HttpGet, ApiAuth]
        public UserListViewModel Get([FromUri]UserListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new UserListInputModel();

            var filter = new UserFilter();
            _mapper.Map(inputModel, filter);

            var users = UserService.GetUsers(filter, inputModel.CurrentPage, inputModel.NumPerPage);
            return new UserListViewModel(users);
        }

        /// <summary>
        /// Mark user as deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ApiAuth]
        public BaseResponseModel Delete(int id)
        {
            var vm = new BaseResponseModel();

            // Get existing user
            var user = UserService.GetUserById(id);
            if (user == null)
            {
                throw new HttpException(404, "User not found.");
            }

            // Check permissions
            if (!CurrentUser.HasPermission(Permission.EditUsers))
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            UserService.DeleteUser(user);

            LogService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    User = CurrentUser,
                    Level = LogLevel.Info,
                    Message = "User " + user.Email + " (ID #" + user.Id + ") was deleted."
                });

            return new BaseResponseModel
                {
                    Success = true
                };
        }

        /// <summary>
        /// Generates a new token that will allow the user to update their password
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseModel ForgotPassword(ForgotPasswordInputModel inputModel)
        {
            // Get existing user
            var vm = new BaseResponseModel();
            var user = UserService.GetUserByEmail(inputModel.Email);
            if (user != null)
            {
                UserService.GenerateResetRequest(user);
                vm.Success = true;
            }

            return vm;
        }

        /// <summary>
        /// Updates a user's current password to the specified new password
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseModel ResetPassword(ResetPasswordInputModel inputModel)
        {
            // Get existing user
            var vm = new BaseResponseModel();
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            if (validationState.IsValid)
            {
                var user = UserService.GetUserByResetToken(inputModel.ResetToken);
                if (user != null)
                {
                    UserService.ResetPassword(user, inputModel.Password);
                    vm.Success = true;
                }
                else
                {
                    validationState.AddError("ResetToken", "Invalid reset token.");
                }
            }

            vm.Errors = validationState.Errors;

            return vm;
        }

        /// <summary>
        /// Add a new user in the system. Only Members roles may be added this way,
        /// other roles must be added within the Manage area.
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseModel SignUp(SignUpInputModel inputModel)
        {
            var vm = new AuthResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            var user = new User
                {
                    Email = inputModel.Email,
                    Role = Role.Member
                };
            UserService.GenerateUserPassword(user, inputModel.Password);

            if (UserService.ValidateUser(user, validationState))
            {
                UserService.CreateUser(user);

                // Authenticate to create token
                user = UserService.Authenticate(user.Email, inputModel.Password);
                _auth.DoAuth(user.Email, false);

                vm.Success = true;
                vm.Token = user.AuthToken.Token;
                vm.Expires = user.AuthToken.Expires;
            }

            vm.Errors = validationState.Errors;
            return vm;
        }

        /// <summary>
        /// Attempt to sign a user into the system
        /// </summary>
        /// <param name="inputModel">Sign in parameters</param>
        /// <returns>Sign in success/failure information</returns>
        [HttpPost]
        public BaseResponseModel SignIn(SignInInputModel inputModel)
        {
            var vm = new AuthResponseModel();
            var user = UserService.Authenticate(inputModel.Email, inputModel.Password);
            if (user != null)
            {
                _auth.DoAuth(inputModel.Email, inputModel.RememberMe);
                vm.Success = true;
                vm.Token = user.AuthToken.Token;
                vm.Expires = user.AuthToken.Expires;
            } else
            {
                vm.Errors.Add("Invalid email or password");

                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Security,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "Authentication failed using email: " + inputModel.Email
                });
            }

            return vm;
        }

        /// <summary>
        /// Sign user out of the system
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseModel SignOut()
        {
            _auth.SignOut();

            // @todo - API is not stateless.
            // The below implementation prevents the API from being
            // stateless. A better implementation would be OAuth or some other
            // kerberos/token method, however for the time being...

            // clear authentication cookie
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            authCookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(authCookie);
            
            // clear session cookie
            var sessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            sessionCookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(sessionCookie);

            var vm = new BaseResponseModel
            {
                Success = true
            };
            return vm;
        }

        /// <summary>
        /// Update a user's information.
        /// </summary>
        /// <param name="inputModel">Info to update</param>
        /// <returns></returns>
        [HttpPost, ApiAuth]
        public BaseResponseModel Update(UpdateUserInputModel inputModel)
        {
            var vm = new BaseResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            // Get existing user
            var user = UserService.GetUserById(inputModel.UserId);
            if (user == null)
            {
                throw new HttpException(404, "User not found.");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditUsers)
                && user.Id != CurrentUser.Id)
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            // Copy properties
            bool emailChanged = user.Email != inputModel.Email;
            user.Email = inputModel.Email;
            string newPass = String.IsNullOrWhiteSpace(inputModel.Password)
                ? null : inputModel.Password;

            // Additional properties for admin users
            if (CurrentUser.HasPermission(Permission.EditUsers))
            {
                if (inputModel.Role.HasValue) user.Role = inputModel.Role.Value;
            }

            if (UserService.ValidateUser(user, validationState))
            {
                UserService.UpdateUser(user, newPass);
                if (emailChanged)
                {
                    ReAuthorizeUser(inputModel.Email);
                }

                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "User " + inputModel.Email + " (ID #" + user.Id + ") was updated.",
                    User = CurrentUser
                });

                vm.Success = true;
            }

            vm.Errors = validationState.Errors;
            return vm;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Log a user back into the system after their email has changed.
        /// </summary>
        /// <param name="email"></param>
        private void ReAuthorizeUser(string email)
        {
            bool rememberMe = false;

            // Retrieve "remember me" cookie
            var cookieName = FormsAuthentication.FormsCookieName;
            var request = HttpContext.Current.Request;
            var cookie = request.Cookies.Get(cookieName);
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                rememberMe = ticket.IsPersistent;
            }
            _auth.DoAuth(email, rememberMe);
        }

        #endregion
    }

}
