using System.Collections.Generic;
using Honeypot.Services;
using Honeypot.Domain;

namespace Honeypot.Web.Areas.Api.Models
{
    public class ResourceListViewModel : PagedViewModel<Resource>
    {
        #region Constructor

        public ResourceListViewModel(IPagedList<Resource> resources)
            : base(resources)
        {
            Resources = new List<ResourceViewModel>();
            foreach (Resource resource in resources.Items)
            {
                Resources.Add(new ResourceViewModel(resource));
            }
        }

        #endregion

        #region Public Properties

        public IList<ResourceViewModel> Resources { get; set; }

        #endregion
    }
}