using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class ForgotPasswordInputModel : BaseInputModel
    {
        public string Email { get; set; }
    }
}