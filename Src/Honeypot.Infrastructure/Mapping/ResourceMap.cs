using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Honeypot.Domain;

namespace Honeypot.Infrastructure.Mapping
{
    public class ResourceMap : IAutoMappingOverride<Resource>
    {
        public void Override(AutoMapping<Resource> mapping)
        {
            mapping.Table("Resources");
            mapping.Id(x => x.Id, "ResourceID");
            mapping.Map(x => x.Name).Not.Nullable().Unique().Length(150);
            mapping.Map(x => x.Value);
            mapping.Map(x => x.IsReadOnly);
            mapping.Map(x => x.Category).Not.Nullable().CustomType<ResourceCategory>();
            mapping.Map(x => x.Type).Not.Nullable().CustomType<ResourceType>();
        }
    }
}
