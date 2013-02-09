using Honeypot.Domain;
using Honeypot.Domain.Filters;

namespace Honeypot.Services
{
    
    public interface IUserService
    {
        User Authenticate(string token);
        User Authenticate(string username, string password);
        void CreateUser(User user);
        void DeleteUser(User user);
        void GenerateUserPassword(User user, string plainTextPassword);
        void GenerateResetRequest(User user);
        User GetUserById(int id);
        User GetUserByEmail(string email);
        User GetUserByResetToken(string token);
        IPagedList<User> GetUsers(UserFilter filter, int currentPage, int numPerPage);
        void ResetPassword(User user, string newPassword);
        void UpdateUser(User user, string newPassword = null);
        bool ValidateUser(User user, IValidationDictionary validationDictionary);
    }
}
