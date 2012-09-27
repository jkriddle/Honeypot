using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CFC.Services
{
    public class EmailHelper
    {
        public static bool IsValid(string email)
        {
            const string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z][a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
            var match = Regex.Match(email.Trim(), pattern, RegexOptions.IgnoreCase);
            return match.Success;
        }
    }
}
