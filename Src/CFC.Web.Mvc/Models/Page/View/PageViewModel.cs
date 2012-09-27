namespace CFC.Web.Mvc.Models.Page
{
    public class PageViewModel : BaseViewModel
    {
        #region Fields

        private readonly Domain.Page _innerPage;

        #endregion

        #region Constructor

        public PageViewModel()
        {
        }

        public PageViewModel(Domain.Page page)
        {
            _innerPage = page;
        }

        #endregion

        #region Public Properties

        public int Id
        {
            get { return _innerPage.Id; }
        }

        public string Title
        {
            get { return _innerPage.Title; }
        }

        public string Content
        {
            get { return _innerPage.Content; }
        }

        public string Name
        {
            get { return _innerPage.Name; }
        }

        #endregion
    }
}