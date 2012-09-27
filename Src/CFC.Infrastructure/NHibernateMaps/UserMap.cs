using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using CFC.Domain;

namespace CFC.Infrastructure.NHibernateMaps
{
    public class UserMap : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.Table("Users");
            mapping.Id(x => x.Id, "UserID");
            mapping.Map(x => x.FirstName).Not.Nullable().Length(50);
            mapping.Map(x => x.LastName).Not.Nullable().Length(50);
            mapping.Map(x => x.Email).Not.Nullable().Unique().Length(150);
            mapping.Map(x => x.State).Length(2);
            mapping.Map(x => x.CellPhone).Length(10);
            mapping.Map(x => x.Carrier).Length(255);
            mapping.Map(x => x.HashedPassword).Not.Nullable();
            mapping.Map(x => x.Salt).Not.Nullable();
            mapping.Map(x => x.Role).Not.Nullable();
            mapping.Map(x => x.ResetPasswordToken).Length(16);
            mapping.Map(x => x.FacebookId).Length(20);
            mapping.Map(x => x.AccessToken).Length(255);
            mapping.Map(x => x.AuthToken).Length(255);
            mapping.Map(x => x.SmS);
            mapping.Map(x => x.Image);
        }
    }
}
