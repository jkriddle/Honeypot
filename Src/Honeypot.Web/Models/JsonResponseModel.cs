using System.Collections.Generic;
using Honeypot.Services;

namespace Honeypot.Web.Models
{
    public class JsonResponseModel
    {
        public IList<ValidationError> Errors { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public bool IsValid
        {
            get { return Errors.Count == 0; }
        }

        public JsonResponseModel()
        {
            Errors = new List<ValidationError>();
        }
    }
}