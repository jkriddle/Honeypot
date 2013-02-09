using System;
using System.Collections.Generic;
using Honeypot.Domain.Filters;
using NHibernate.Criterion;
using Honeypot.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Honeypot.Infrastructure
{
    public class AuthTokenRepository : LinqRepository<AuthToken>, IAuthTokenRepository
    {
    }
}
