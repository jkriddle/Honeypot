using Honeypot.Domain;

namespace Honeypot.Web.Areas.Api.Models
{
    public class UserListInputModel : PagedInputModel
    {
        public string Email { get; set; }
        public Role? Role { get; set; }
    }
}