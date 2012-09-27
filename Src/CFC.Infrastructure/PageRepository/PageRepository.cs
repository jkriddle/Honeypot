using CFC.Domain;
using SharpArch.NHibernate;

namespace CFC.Infrastructure.PageRepository
{
    public class PageRepository : LinqRepository<Page>, IPageRepository
    {
    }
}
