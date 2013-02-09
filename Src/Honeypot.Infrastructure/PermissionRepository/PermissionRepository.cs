using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using Honeypot.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Honeypot.Infrastructure
{
    public class PermissionRepository : LinqRepository<Permission>, IPermissionRepository
    {
        
    }
}
