using System.ComponentModel.DataAnnotations;

namespace CFC.Web.Mvc.Models.Login
{
    public class ForgotPasswordInputModel : BaseInputModel
    {
        [Required]
        public string Email { get; set; }
    }
}