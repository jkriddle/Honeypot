using Honeypot.Domain;

namespace Honeypot.Web.Areas.Api.Models
{
    public class ResourceListInputModel : PagedInputModel
    {
        public ResourceCategory? ResourceCategory { get; set; }
        public ResourceType? ResourceType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool? IsReadOnly { get; set; }
    }
}