using System;
using NHibernate.Spatial.Type;

namespace Honeypot.Infrastructure.Mapping.Conventions
{
[Serializable]
    public class GeographyType : MsSql2008GeographyType
    {
        protected override void SetDefaultSRID(GeoAPI.Geometries.IGeometry geometry)
        {
            geometry.SRID = 4326;
        }
    }
}
