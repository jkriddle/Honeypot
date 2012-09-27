using System.Collections.Generic;

namespace CFC.Web.Mvc.Models.Page
{
    public class PageListViewModel : BaseViewModel
    {
        #region Constructor

        public PageListViewModel(IEnumerable<Domain.Page> pages)
        {
            Pages = new List<PageViewModel>();
            foreach (Domain.Page p in pages)
            {
                Pages.Add(new PageViewModel(p));
            }
        }

        #endregion

        #region Public Properties

        public IList<PageViewModel> Pages { get; set; }

        #endregion
    }
}