using System.Net.Http;
using System.Web;
using System.Web.Http;
using Honeypot.Domain;
using Honeypot.Services;

namespace Honeypot.Web.Areas.Api.Controllers
{
    public class BaseController : ApiController
    {
        #region Fields

        protected readonly IUserService UserService;
        protected readonly ILogService LogService;

        #endregion

        #region Constructor

        public BaseController(IUserService userService, ILogService logService)
        {
            UserService = userService;
            LogService = logService;
        }

        #endregion

        #region Public Properties

        public User CurrentUser { get; set; }

        #endregion

        #region Public Methods

        public string GetClientIp(HttpRequestMessage request)
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null) 
                return null;

            // @note This will not work in a "self hosted" environment or
            // if the API is separated from the MVC framework.
            // If self hosted, use the commented lines below instead.
            // @see http://stackoverflow.com/questions/9565889/get-the-ip-address-of-the-remote-host
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #region Self-Hosted Method

            /*if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)this.Request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else
            {
                return null;
            }*/
            
            #endregion

        }

        #endregion

        #region Overrides

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                string username = HttpContext.Current.User.Identity.Name;
                CurrentUser = UserService.GetUserByEmail(username);
            }
        }

        #endregion

    }
}