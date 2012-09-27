namespace MEF.FacebookPlugin
{
    public interface IFacebookApi
    {
        string GetAccessToken(string appId, string appSecret, string code, string redirectUrl);
        UserProfile GetCurrentUserProfile();
    }
}
