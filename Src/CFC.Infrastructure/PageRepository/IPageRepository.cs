using CFC.Domain;
using SharpArch.Domain.PersistenceSupport;

namespace CFC.Infrastructure.PageRepository
{
    public interface IPageRepository : ILinqRepositoryWithTypedId<Page, int>
    {
    }
    
}
