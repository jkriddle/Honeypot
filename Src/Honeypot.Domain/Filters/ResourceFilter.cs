using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Honeypot.Domain.Filters
{
    public class ResourceFilter : BaseFilter
    {
        public ResourceFilter()
        {
            SortColumn = "Name";
        }
        
        public ResourceCategory? ResourceCategory { get; set; }
        public ResourceType? ResourceType { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool? IsReadOnly { get; set; }
    }
}
