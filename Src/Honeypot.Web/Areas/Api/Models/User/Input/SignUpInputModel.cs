using Honeypot.Services;
using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class SignUpInputModel : BaseInputModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Validate form-level request
        /// </summary>
        /// <param name="validationDictionary"></param>
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();
            if (Password != ConfirmPassword)
            {
                requestDictionary.AddError("Password", "Passwords do not match.");
            }
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}