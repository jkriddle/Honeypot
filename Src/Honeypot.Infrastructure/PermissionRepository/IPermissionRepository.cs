using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Honeypot.Domain;
using SharpArch.Domain.PersistenceSupport;

namespace Honeypot.Infrastructure
{
    public interface IPermissionRepository : ILinqRepositoryWithTypedId<Permission, int>
    {
    }
}
