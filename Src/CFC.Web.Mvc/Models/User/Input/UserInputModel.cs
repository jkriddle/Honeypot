using System.ComponentModel;
using System.Web.Mvc;
using CFC.Domain;
using CFC.Web.Mvc.Attributes;

namespace CFC.Web.Mvc.Models.User
{
    public class UserInputModel : BaseInputModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string CellPhone { get; set; }
        public int ServiceAreaDistance { get; set; }

        [DisplayName("Confirm Password"), Compare("Password")]
        public string ConfirmPassword { get; set; }

        [EnumRequired(ErrorMessage="Role is required.")]
        public Role Role { get; set; }

        public string Carrier { get; set; }
    }
}