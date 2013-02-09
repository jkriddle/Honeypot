using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Honeypot.Domain;
using Honeypot.Domain.Filters;
using SharpArch.Domain.PersistenceSupport;

namespace Honeypot.Infrastructure
{
    public interface IAuthTokenRepository : ILinqRepositoryWithTypedId<AuthToken, int>
    {
    }
}
