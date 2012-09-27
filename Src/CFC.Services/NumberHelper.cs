using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFC.Services
{
    public class NumberHelper
    {
        public static bool IsNumeric(string val, System.Globalization.NumberStyles numberStyle)
        {
            Double result;
            return Double.TryParse(val, numberStyle,
                System.Globalization.CultureInfo.CurrentCulture, out result);
        }
    }
}
