using System;
using System.Collections.Generic;
using Honeypot.Domain.Filters;
using NHibernate.Criterion;
using Honeypot.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Honeypot.Infrastructure
{
    public class ResourceRepository : LinqRepository<Resource>, IResourceRepository
    {
        /// <summary>
        /// Search resources
        /// </summary>
        /// <param name="filter">Parameters to filter resources by</param>
        /// <param name="page">Current page</param>
        /// <param name="numPerPage">Number of records per page</param>
        /// <param name="totalRecords">Total records ignoring paging</param>
        /// <returns></returns>
        public IEnumerable<Resource> Search(ResourceFilter filter, int page, int numPerPage, out int totalRecords)
        {
            Resource resourceAlias= null;

            var query = Session.QueryOver(() => resourceAlias);

            // filter search term
            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<User>(u => resourceAlias.Name).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<User>(u => resourceAlias.Value).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
            }
            
            // filter category
            if (filter.ResourceCategory.HasValue)
            {
                query = query.Where(r => resourceAlias.Category == filter.ResourceCategory.Value);
            }
            
            // filter type
            if (filter.ResourceType.HasValue)
            {
                query = query.Where(r => resourceAlias.Type == filter.ResourceType.Value);
            }

            // filter name
            if (!String.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(Restrictions.On<User>(u => resourceAlias.Name).IsLike(filter.Name, MatchMode.Anywhere));
            }

            // filter value
            if (!String.IsNullOrEmpty(filter.Value))
            {
                query = query.Where(Restrictions.On<User>(u => resourceAlias.Value).IsLike(filter.Value, MatchMode.Anywhere));
            }

            // read only
            if (filter.IsReadOnly.HasValue)
            {
                query = query.Where(r => resourceAlias.IsReadOnly == filter.IsReadOnly);
            }

            query.TransformUsing(Transformers.DistinctRootEntity);

            int firstResult = (page * numPerPage) - numPerPage;
            totalRecords = query.RowCount();

            // Sort
            if (filter.SortDirection == SortDirection.Ascending)
                query = query.OrderBy(Projections.Property(filter.SortColumn)).Asc;
            else
                query = query.OrderBy(Projections.Property(filter.SortColumn)).Desc;

            return query.Skip(firstResult).Take(numPerPage).List();
        }
    }
}
