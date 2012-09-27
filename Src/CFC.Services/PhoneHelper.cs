using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CFC.Services
{
    public static class PhoneHelper
    {

        /// <summary>
        /// Strip all non-numeric characters from a phone number.
        /// </summary>
        /// <param name="phone">Raw phone number</param>
        /// <returns>Only the numbers of a phone</returns>
        public static string GetNumbers(string phone)
        {
            if (String.IsNullOrEmpty(phone)) return phone;
            return Regex.Replace(phone, @"[^\d]", String.Empty);
        }

        /// <summary>
        /// Returns true if phone number is valid
        /// </summary>
        /// <param name="phone">Raw phone number</param>
        /// <returns>If phone number is valid</returns>
        public static bool IsValid(string phone)
        {
            if (String.IsNullOrEmpty(phone)) return false;
            return GetNumbers(phone).Length == 10;
        }

    }
}
