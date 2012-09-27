using System.Collections.Generic;
using CFC.Services.Validation;

namespace CFC.Web.Mvc.Models
{
    public class BaseInputModel
    {
        /// <summary>
        /// If accessing site from external URL, AuthToken is sent to validate user credentials
        /// </summary>
        public string AuthToken { get; set; }
    }
}