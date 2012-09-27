using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CFC.Domain;

namespace CFC.Web.Mvc.Models.User
{
    public class EditProfileResponse : JsonResponseModel
    {
        public bool ChangeEmail { get; set; }
        public string Email { get; set; }
    }
}