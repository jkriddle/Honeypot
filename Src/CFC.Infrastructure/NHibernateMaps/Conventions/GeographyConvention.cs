using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using GeoAPI.Geometries;

namespace CFC.Infrastructure.NHibernateMaps.Conventions
{
    [Serializable]
    public class GeographyConvention : IPropertyConvention 
    {
        public void Apply(IPropertyInstance instance)
        {
            if (typeof(IGeometry).IsAssignableFrom(instance.Property.PropertyType))
            {
                instance.CustomType(typeof(GeographyType));
                instance.CustomSqlType("GEOGRAPHY");
            }
        }
    }
}
