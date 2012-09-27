using CFC.Domain;
using CFC.Web.Mvc.Providers;

namespace CFC.Web.Mvc.Models.Utility
{
    public class UtilityViewModel : BaseViewModel
    {
        #region Fields

        private readonly Domain.User _innerUser;
        private readonly ISessionProvider _sessionProvider;

        #endregion

        #region Constructor

        public UtilityViewModel(Domain.User user, ISessionProvider sessionProvider)
        {
            if (user == null)
            {
                _innerUser = new Domain.User();
                return;
            }
            _sessionProvider = sessionProvider;
            _innerUser = user;
            IsLoggedIn = true;
        }

        #endregion

        public bool IsLoggedIn { get; set; }

        public int UserId
        {
            get { return _innerUser.Id; }
        }

        public string Email
        {
            get { return _innerUser.Email; }
        }

        public bool IsRole1
        {
            get { return _innerUser.Role == Role.Role1; }
        }

    }
}