using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using CFC.Domain.Filters;
using CFC.Services.UserService;
using CFC.Services.Validation;
using CFC.Web.Mvc.Attributes;
using CFC.Web.Mvc.Helpers;
using CFC.Web.Mvc.Models;
using CFC.Web.Mvc.Models.User;
using CFC.Domain;
using CFC.Web.Mvc.Providers;
using CFC.Web.Mvc.Resources;
using CFC.Web.Mvc.Wrappers;


namespace CFC.Web.Mvc.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IAuth _authentication;

        public UserController()
        {
            
        }
         

        [RequiresRole(Role.Role1, Role.Role2)]
        public ActionResult List()
        {
            return View("List");
        }

        [RequiresRole(Role.Role1, Role.Role2), NoCache]
        public ActionResult Get(PagedInputModel inputModel)
        {
            var vm = GetUserListViewModel(inputModel);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private UserListViewModel GetUserListViewModel(PagedInputModel inputModel)
        {
            inputModel.SearchQuery = Server.UrlDecode(inputModel.SearchQuery);
            var filter = new UserFilter
            {
                SearchTerm = inputModel.SearchQuery
            };
           
            var users = _userService.GetUsers(filter, inputModel.CurrentPage, inputModel.NumPerPage);
            var vm = new UserListViewModel(users);
            return vm;
        }

        /// <summary>
        /// Retrieve a single user's information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RequiresAuthentication, NoCache]
        public ActionResult GetOne(int id)
        {
            User user = _userService.GetUserById(id);

            if (user == null)
            {
                throw new HttpException(404, "NotFound");
            }

            var vm = new UserViewModel(user);

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Log into the site as a specified user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Emulate(int id)
        {
            var session = new SessionProvider(ControllerContext);
            User user = _userService.GetUserById(id);

            if (user == null)
            {
                throw new HttpException(404, "NotFound");
            }

            session.EmulateUser(CurrentUser.Id);
            _authentication.DoAuth(user.Email, false);

            return RedirectToAction("Profile");
        }


        /// <summary>
        /// If emulating as another user, return to original user.
        /// </summary>
        /// <returns></returns>
        public ActionResult Return()
        {
            var session = new SessionProvider(ControllerContext);
            User user = _userService.GetUserById(session.OriginalUserId);

            if (user == null)
            {
                throw new HttpException(404, "NotFound");
            }

            session.RestoreOriginalUserSession();
            _authentication.DoAuth(user.Email, false);

            return RedirectToAction("List");
        }

        /// <summary>
        /// Display a user's profile
        /// </summary>
        /// <param name="id">Username of user's profile</param>
        /// <returns></returns>
        [RequiresAuthentication, ViewModelUserFilter]
        public ActionResult Profile(int? id = null)
        {
            User user = null;
            user = id.HasValue ? _userService.GetUserById(id.Value) : CurrentUser;
            if (user == null)
            {
                throw new HttpException(404, "NotFound");
            }
            var vm = new ProfileViewModel(user);
            return View("Profile", vm);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="inputModel">Posted data</param>
        /// <returns></returns>
        [HttpPost, ApiAuth, NoCache]
        public ActionResult Create(UserInputModel inputModel)
        {
            var user = new User
                           {
                               FirstName = inputModel.FirstName,
                               LastName = inputModel.LastName,
                               Email = inputModel.Email,
                               CellPhone = inputModel.CellPhone,
                               Role = inputModel.Role,
                            };

            if (inputModel.Role == Role.Role1)
            {
                throw new Exception("Invalid role selection.");
            }

            // Setup password and salt
            _userService.GenerateUserPassword(user, inputModel.Password);

            var validationState = new ModelStateWrapper(ModelState);

            var vm = new JsonResponseModel();

            // If both the view model and the user object are valid
            if (_userService.ValidateUser(user, validationState))
            {
                _userService.CreateUser(user);
                vm.Success = true;
                vm.Message = AppResources.UserCreated;
                return Json(vm, JsonRequestBehavior.AllowGet);
            }

            vm.Errors = validationState.Errors;

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edit a user's profile
        /// </summary>
        /// <returns></returns>
        [RequiresAuthentication, ViewModelUserFilter]
        public ActionResult EditProfile()
        {
            var vm = new EditProfileViewModel(CurrentUser);
            return View("EditProfile", vm);
        }

        /// <summary>
        /// Edit a user's profile
        /// </summary>
        /// <param name="inputModel">Posted data</param>
        /// <returns></returns>
        [HttpPost, ApiAuth, NoCache]
        public ActionResult EditProfile(EditProfileInputModel inputModel)
        {
            User user = CurrentUser;
            string originalEmail = CurrentUser.Email;

            user.FirstName = inputModel.FirstName;
            user.LastName = inputModel.LastName;
            user.Email = inputModel.Email;
            user.State = inputModel.State;
            var vm = new EditProfileResponse();
            var validationState = new ModelStateWrapper(ModelState);

            // If a photo was uploaded, add it to this vehicle
            if (!String.IsNullOrEmpty(inputModel.UserPhotoName))
            {
                string filePath = Path.Combine(Server.MapPath(ConfigHelper.AppSetting("FileUploadPath")), inputModel.UserPhotoName);

                if (System.IO.File.Exists(filePath))
                {
                    user.Image = System.IO.File.ReadAllBytes(filePath);
                }
                else
                {
                    vm.Success = false;
                    vm.Errors.Add(new ValidationError("Image", AppResources.PhotoNotFound));
                }
            }
            
            // If both the view model and the user object are valid
            if (_userService.ValidateUser(user, validationState))
            {
                // If user is updating their password, pass it through.
                string newPassword = null;
                if (!String.IsNullOrEmpty(inputModel.Password))
                {
                    newPassword = inputModel.Password;
                }
                _userService.UpdateUser(user, newPassword);

                // If user email changed, they need to be logged in again.
                if (originalEmail != user.Email)
                {
                    vm.ChangeEmail = true;
                    vm.Email = user.Email;
                    _authentication.DoAuth(user.Email, false);
                }
                vm.Message = AppResources.UserUpdated;
                vm.Success = true;
            }

            vm.Errors = validationState.Errors;
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

    }
}
