using System.Collections.Generic;
using CFC.Services.PagedList;

namespace CFC.Web.Mvc.Models.User
{
    public class UserListViewModel : PagedViewModel<Domain.User>
    {
        #region Constructor

        public UserListViewModel(IPagedList<Domain.User> users)
            : base(users)
        {
            Users = new List<UserViewModel>();
            foreach (Domain.User user in users.Items)
            {
                Users.Add(new UserViewModel(user));
            }
        }

        #endregion

        #region Public Properties

        public IList<UserViewModel> Users { get; set; }

        #endregion
    }
}