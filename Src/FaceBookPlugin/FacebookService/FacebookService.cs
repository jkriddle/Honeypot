

namespace MEF.FacebookPlugin
{

    public class FacebookService : IFacebookService
    {
        private readonly string _appId;
        private readonly string _appKey;

        public FacebookService(string appId, string appKey)
        {
            _appId = appId;
            _appKey = appKey;
        }

        /// <summary>
        /// Retrieve a user access token.
        /// </summary>
        /// <param name="code">Authorization code</param>
        /// <returns>New access token</returns>
        public string GetFacebookAccessToken(string code, string redirectUrl)
        {
            using (var fb = new FacebookApi())
            {
                return fb.GetAccessToken(_appId, _appKey, code, redirectUrl);
            }
        }

        /// <summary>
        /// Retrieve a user's Facebook profile
        /// </summary>
        /// <param name="accessToken">User's access token</param>
        /// <returns></returns>
        public UserProfile GetFacebookProfile(string accessToken)
        {
            using (var fb = new FacebookApi(accessToken))
            {
                return fb.GetCurrentUserProfile();
            }
        }

    }
}
