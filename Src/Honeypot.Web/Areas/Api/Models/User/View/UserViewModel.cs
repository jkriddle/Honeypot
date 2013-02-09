using Honeypot.Domain;
using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class UserViewModel : BaseViewModel
    {
        #region Fields

        private readonly User _innerUser;

        #endregion

        #region Constructor

        public UserViewModel(User user)
        {
            _innerUser = user;
        }

        #endregion

        #region Properties

        public int UserId { get { return _innerUser.Id; } }
        public string Email { get { return _innerUser.Email; } }
        public string Role { get { return _innerUser.Role.GetDescription(); } }

        #endregion


    }
}