using CFC.Web.Mvc.Providers;

namespace CFC.Web.Mvc.Models.User
{
    public class ProfileViewModel : BaseViewModel
    {
        #region Fields

        private readonly Domain.User _innerUser;

        #endregion

        #region Constructor

        public ProfileViewModel(Domain.User user)
        {
            _innerUser = user;
        }

        #endregion

        #region Public Properties

        public int Id
        {
            get { return _innerUser.Id; }
        }

        public string FullName
        {
            get { return _innerUser.FullName; }
        }

        public string Email
        {
            get { return _innerUser.Email; }
        }

        public string CellPhone
        {
            get { return _innerUser.CellPhone; }
        }

        public bool HasImage
        {
            get { return _innerUser.Image != null; }
        }

        public bool CanEdit
        {
            get
            {
                // Can only edit their own profile
                return _innerUser.Id == CurrentUserId;
            }
        }

        #endregion
    }
}