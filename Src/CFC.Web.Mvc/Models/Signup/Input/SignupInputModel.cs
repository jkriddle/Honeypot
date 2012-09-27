using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;
using System.Web.Mvc;
using CFC.Services.UserService;
using CFC.Services.Validation;
using CFC.Web.Mvc.Attributes;
using CFC.Web.Mvc.Wrappers;

namespace CFC.Web.Mvc.Models.Signup
{
    public class SignupInputModel : JsonResponseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string Password { get; set; }
        public string CellPhone { get; set; }

        [DisplayName("Confirm Password"), Compare("Password")]
        public string ConfirmPassword { get; set; }

        [BooleanRequired(ErrorMessage = "You must agree to the terms of service to sign up.")]
        public bool AgreeToTerms { get; set; }
    }
}