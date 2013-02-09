using Honeypot.Domain;
using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class ResourceViewModel : BaseViewModel
    {
        #region Fields

        private readonly Resource _innerResource;

        #endregion

        #region Constructor

        public ResourceViewModel(Resource resource)
        {
            _innerResource = resource;
        }

        #endregion

        #region Properties

        public int ResourceId { get { return _innerResource.Id; } }
        public string Name { get { return _innerResource.Name; } }
        public string Category { get { return _innerResource.Category.GetDescription(); } }
        public string Type { get { return _innerResource.Type.GetDescription(); } }
        public string Value { get { return _innerResource.Value; } }
        public bool IsReadOnly { get { return _innerResource.IsReadOnly; } }

        #endregion


    }
}