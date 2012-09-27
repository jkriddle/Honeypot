using System.Reflection;
using CFC.Domain;

using CFC.Domain.Filters;
using CFC.Services.PagedList;
using CFC.Services.Validation;
using MEF.FacebookPlugin;


namespace CFC.Services.UserService
{
    
    public interface IUserService
    {
       
        User Authenticate(string authToken);
        User Authenticate(string username, string password, string hardwareId = null);
        
        bool AuthenticateFacebookUser(UserProfile profile);
        void CreateUser(User user);
        string GenerateRandomPassword(int passwordLength = 16, string whitelistChars = "");
        void GenerateUserPassword(User user, string plainTextPassword);
        IPagedList<User> GetUsers(UserFilter filter, int currentPage, int numPerPage);
        User GetUserByEmail(string email);
        User GetUserByFacebookId(long facebookId);
        User GetUserById(int id);
        User GetUserByResetToken(string token);
        bool GenerateForgotPasswordLink(string resetRootUrl, string email);
        User RegisterFacebookUser(UserProfile profile);
        bool ResetPassword(string token, string newPassword);
        void UpdateUser(User user, string newPassword = null);
        bool ValidateUser(User user, IValidationDictionary validationDictionary);
    }
}
