using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFC.Domain;
using CFC.Domain.Filters;
using SharpArch.Domain.PersistenceSupport;

namespace CFC.Infrastructure.UserRepository
{
    public interface IUserRepository : ILinqRepositoryWithTypedId<User, int>
    {
        IEnumerable<User> Search(UserFilter filter, int page, int numPerPage, ref int totalRecords);
    }
}
