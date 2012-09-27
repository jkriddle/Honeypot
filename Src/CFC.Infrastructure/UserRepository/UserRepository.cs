using System;
using System.Collections.Generic;
using CFC.Domain.Filters;
using NHibernate.Criterion;
using CFC.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace CFC.Infrastructure.UserRepository
{
    public class UserRepository : LinqRepository<User>, IUserRepository
    {
        public IEnumerable<User> Search(UserFilter filter, int page, int numPerPage, ref int totalRecords)
        {
            User userAlias = null;
           

            var query = Session.QueryOver(() => userAlias);

            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<User>(u => userAlias.FirstName).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<User>(u => userAlias.LastName).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<User>(u => userAlias.Email).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<User>(u => userAlias.CellPhone).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
            }

            query.TransformUsing(Transformers.DistinctRootEntity);

            int firstResult = (page * numPerPage) - numPerPage;
            totalRecords = query.RowCount();

            return query.Skip(firstResult).Take(numPerPage).List();
        }
    }
}
