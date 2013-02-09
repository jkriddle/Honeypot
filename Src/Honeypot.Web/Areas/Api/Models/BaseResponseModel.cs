using System.Collections.Generic;

namespace Honeypot.Web.Areas.Api.Models
{
    public class BaseResponseModel
    {
        public bool Success { get; set; }
        public IList<string> Errors { get; set; }

        public BaseResponseModel()
        {
            Errors = new List<string>();
        }
    }
}