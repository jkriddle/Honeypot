using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using SharpArch.Domain.PersistenceSupport;

namespace Honeypot.Infrastructure
{
    public interface ILogRepository : ILinqRepositoryWithTypedId<Log, int>
    {
        IEnumerable<Log> Search(LogFilter filter, int page, int numPerPage, out int totalRecords);
    }
}
