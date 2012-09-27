using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CFC.Web.Mvc.Models.Login
{
    public class LoginInputModel : BaseInputModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string HardwareId { get; set; }
    }
}