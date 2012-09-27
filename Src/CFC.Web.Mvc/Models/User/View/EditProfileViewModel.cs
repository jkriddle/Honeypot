using System;
using System.Web.Mvc;
using CFC.Web.Mvc.Helpers;

namespace CFC.Web.Mvc.Models.User
{
    public class EditProfileViewModel : BaseViewModel
    {
        #region Fields

        private readonly Domain.User _innerUser;

        #endregion

        #region Constructor

        public EditProfileViewModel(Domain.User user)
        {
            _innerUser = user;
        }

        #endregion

        #region Public Properties
        
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

        public bool HasImage
        {
            get { return _innerUser.Image != null; }
        }

        public string State
        {
            get { return _innerUser.State; }
        }

        public string CellPhone
        {
            get { return _innerUser.CellPhone; }
        }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public SelectList StateList
        {
            get { return StateListHelper.StateSelectListWithValue(State); }
        }

        #endregion
    }
}