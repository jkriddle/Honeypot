using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class SignInInputModel : BaseInputModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}