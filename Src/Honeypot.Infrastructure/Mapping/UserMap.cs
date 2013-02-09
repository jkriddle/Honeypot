using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Honeypot.Domain;

namespace Honeypot.Infrastructure.Mapping
{
    public class UserMap : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.Table("Users");
            mapping.Id(x => x.Id, "UserID");
            mapping.Map(x => x.Email).Not.Nullable().Unique().Length(150);
            mapping.Map(x => x.HashedPassword).Not.Nullable();
            mapping.Map(x => x.Salt).Not.Nullable();
            mapping.Map(x => x.Role).Not.Nullable().CustomType<Role>();
            mapping.References(x => x.AuthToken).Column("AuthTokenID").Cascade.All();
            mapping.Map(x => x.Status).Not.Nullable().CustomType<UserStatus>();
            mapping.HasMany(x => x.Permissions);
            mapping.HasMany(x => x.Logs);
        }
    }
}
