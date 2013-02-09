using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Honeypot.Domain;
using Honeypot.Domain.Filters;

namespace Honeypot.Services
{
    public interface IResourceService
    {
        PagedList<Resource> GetResources(ResourceFilter filter, int currentPage, int numPerPage);
        Resource GetResourceById(int id);
        Resource GetResourceByName(string name);
        void CreateResource(Resource resource);
        void UpdateResource(Resource resource);
        bool ValidateResource(Resource resource, IValidationDictionary validationDictionary);
    }

}
