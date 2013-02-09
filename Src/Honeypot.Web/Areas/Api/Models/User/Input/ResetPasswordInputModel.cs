using Honeypot.Services;
using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class ResetPasswordInputModel : BaseInputModel
    {
        public string ResetToken { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Validate form-level request
        /// </summary>
        /// <param name="validationDictionary"></param>
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();
            if (string.IsNullOrEmpty(ResetToken))
            {
                requestDictionary.AddError("ResetToken", "Reset token not specified.");
            }
            if (string.IsNullOrEmpty(Password))
            {
                requestDictionary.AddError("Password", "Password is required.");
            }
            if (Password != ConfirmPassword)
            {
                requestDictionary.AddError("Password", "Passwords do not match.");
            }
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}