using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class AuthResponseModel : BaseResponseModel
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}