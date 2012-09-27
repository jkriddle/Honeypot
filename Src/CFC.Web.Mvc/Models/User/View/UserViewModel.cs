using CFC.Domain;


namespace CFC.Web.Mvc.Models.User
{
    public class UserViewModel : BaseViewModel
    {
        #region Fields

        private readonly Domain.User _innerUser;

        #endregion

        #region Constructor

        public UserViewModel(Domain.User user)
        {
            _innerUser = user;
        }

        #endregion

        #region Public Propreties

        public int Id
        {
            get { return _innerUser.Id; }
        }

        public string FirstName
        {
            get { return _innerUser.FirstName; }
        }

        public string LastName
        {
            get { return _innerUser.LastName; }
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

        public string RoleName
        {
            get { return _innerUser.Role.GetDescription(); }
        }

        public string Carrier
        {
            get { return _innerUser.Carrier; }
        }

        #endregion
    }
}