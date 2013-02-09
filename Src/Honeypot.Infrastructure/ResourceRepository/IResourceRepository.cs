using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using SharpArch.Domain.PersistenceSupport;

namespace Honeypot.Infrastructure
{
    public interface IResourceRepository : ILinqRepositoryWithTypedId<Resource, int>
    {
        IEnumerable<Resource> Search(ResourceFilter filter, int page, int numPerPage, out int totalRecords);
    }
}
