using Honeypot.Domain;
using Honeypot.Services;
using Honeypot.Web.Models;

namespace Honeypot.Web.Areas.Api.Models
{
    public class UpdateResourceInputModel : BaseInputModel
    {
        public int ResourceId { get; set; }
        public string Value { get; set; }
    }
}