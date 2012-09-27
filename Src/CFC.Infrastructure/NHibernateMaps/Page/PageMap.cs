using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using CFC.Domain;

namespace CFC.Infrastructure.NHibernateMaps
{
    public class PageMap : IAutoMappingOverride<Domain.Page>
    {
        public void Override(AutoMapping<Domain.Page> mapping)
        {
            mapping.Table("Pages");
            mapping.Id(x => x.Id, "PageID"); 
            mapping.Map(x => x.Name);
            mapping.Map(x => x.Title);
            mapping.Map(x => x.Content);
        }
    }
}
