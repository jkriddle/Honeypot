using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Honeypot.Domain;

namespace Honeypot.Infrastructure.Mapping
{
    public class PermissionMap : IAutoMappingOverride<Permission>
    {
        public void Override(AutoMapping<Permission> mapping)
        {
            mapping.Table("Permissions");
            mapping.Id(x => x.Id, "PermissionID");
            mapping.Map(x => x.Name).Not.Nullable().Unique().Length(150);
            mapping.Map(x => x.Description).Not.Nullable();
        }
    }
}
