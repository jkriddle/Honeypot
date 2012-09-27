using System;
using System.Runtime.Serialization;
using System.Web;

namespace CFC.Web.Mvc.Models.Login
{
    public class FacebookAuthInputModel
    {
        private HttpRequest _request
        {
            get { return HttpContext.Current.Request; }
        }

        public string Code { get { return _request.Params["code"]; } }

        public string State { get { return _request.Params["state"]; } }

        public string Error { get { return _request.Params["error"]; } }

        public string ErrorReason { get { return _request.Params["error_reason"]; } }

        public string ErrorDescription { get { return _request.Params["error_description"]; } }

        /// <summary>
        /// If user authorized access to Facebook
        /// </summary>
        public bool IsAuthorized { get { return !String.IsNullOrEmpty(Code); }}
    }
}