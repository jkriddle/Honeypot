using System;
using System.Linq;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace CFC.Domain
{
    public static class PointConversion
    {

        /// <summary>
        /// The SQL implementation of a POINT is REVERSED from what the SQL server geography data type uses.
        /// Thus we have to create a new point using POINT(longitude, latitude) instead of
        /// the expected POINT(latitude, longitude)
        /// </summary>
        /// <param name="latLong"></param>
        /// <returns></returns>
        public static Point ConvertString(string latLong)
        {
            if (latLong == null) return new Point(0,0);
            var s = latLong.Split(',');
            return s.Count() == 2 ? new Point(Convert.ToDouble(s[1]), Convert.ToDouble(s[0])) : null;
        }

        /// <summary>
        /// Convert comma separated lat/long values (i.e. "lat, long") to a format of "long lat" for use
        /// in the SQL geometry point parameter.
        /// </summary>
        /// <param name="latLong"></param>
        /// <returns></returns>
        public static string PointToSqlPoint(Point point)
        {
            return point.Y + " " + point.X;
        }

        /// <summary>
        /// Convert comma separated lat/long values (i.e. "lat, long") to a format of "long lat" for use
        /// in the SQL geometry point parameter.
        /// </summary>
        /// <param name="latLong"></param>
        /// <returns></returns>
        public static string PointToCommaValues(Point point)
        {
            return point.Y + ", " + point.X;
        }

        /// <summary>
        /// Convert comma separated lat/long values (i.e. "lat, long") to a format of "long lat" for use
        /// in the SQL geometry point parameter.
        /// </summary>
        /// <param name="latLong"></param>
        /// <returns></returns>
        public static string CommaValuesToSqlPoint(string latLong)
        {
            Point point = ConvertString(latLong);
            return point.X + " " + point.Y;
        }
    }
}