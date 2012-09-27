namespace MEF.FacebookPlugin
{
    public interface IFacebookService
    {
        string GetFacebookAccessToken(string code, string redirectUrl);
        UserProfile GetFacebookProfile(string accessToken);
    }
}
