using System;
using Facebook;
namespace MEF.FacebookPlugin
{
    public class FacebookApi : IFacebookApi, IDisposable
    {
        private FacebookClient _client;


        public FacebookApi()
        {
        }

        public FacebookApi(string authToken)
        {
            _client = new FacebookClient(authToken);
        }

        public string GetAccessToken(string appId, string appSecret, string accessCode, string redirectUrl)
        {
            var fb = new FacebookClient();

            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = appId,
                client_secret = appSecret,
                redirect_uri = redirectUrl,
                code = accessCode
            });

            return result.access_token;
        }

        public UserProfile GetCurrentUserProfile()
        {
            dynamic userProfile = _client.Get("me");
            return new UserProfile
            {
                FirstName = userProfile.first_name,
                LastName = userProfile.last_name,
                Email = userProfile.email,
                FacebookId = long.Parse(userProfile.id),
                AccessToken = _client.AccessToken
            };
        }

        public void Dispose()
        {
            _client = null;
        }
    }
}
