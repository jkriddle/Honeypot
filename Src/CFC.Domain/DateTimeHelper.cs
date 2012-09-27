using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFC.Domain
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Converts a Date/Time to a specific timezone. 
        /// CURRENTLY HARD CODED TO CONVERT TO EASTERN UNTIL OFFSETS ARE STORED.
        /// </summary>
        /// <param name="dt">UTC Date/time to convert</param>
        /// <returns></returns>
        public static DateTime ConvertUtcToTimeZone(DateTime dt)
        {
            // Ensure date being passed is recognized as UTC
            DateTime utc = new DateTime(dt.Ticks, DateTimeKind.Utc);

            //@todo implement time zone as a parameter instead of hardcoding
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); 

            return TimeZoneInfo.ConvertTime(utc, tz);
        }
    }
}
