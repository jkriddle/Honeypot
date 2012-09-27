using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CFC.Web.Mvc.Models.Login
{
    public class ResetPasswordInputModel : BaseInputModel
    {
        [Required, DisplayName("Reset Token")]
        public string Token { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, DisplayName("Confirm Password"), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}