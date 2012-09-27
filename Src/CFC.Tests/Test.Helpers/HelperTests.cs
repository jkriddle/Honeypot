using System;
using System.Globalization;
using System.Linq;
using CFC.Services;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;

namespace CFC.Tests.CFC.Misc
{
    [TestFixture]
    public class HelperTests
    {
     
        [Test]
        public void Email_Helper_True_For_Valid_Email()
        {
            Assert.IsTrue(EmailHelper.IsValid("test@mailinator.com"), "Valid Email Failed to Validate");
        }
        [Test]
        public void Email_Helper_False_For_Invalid_Email()
        {
            Assert.IsFalse(EmailHelper.IsValid("BlahBlahBlah"), "Invalid email address passed validation");
        }

        [Test]
        public void Number_Helper_True_For_Numeric()
        {
            Assert.IsTrue(NumberHelper.IsNumeric("1", NumberStyles.Integer), "Numeric value failed to validate");
        }

        [Test]
        public void Number_Helper_False_For_Non_Numeric()
        {
            Assert.IsFalse(NumberHelper.IsNumeric("one", NumberStyles.Integer), "Non numeric value passed as numeric");
        }

        [Test]
        public void ConvertToPoint()
        {
            var latLong = "39.6565,79.6565";
            var s = latLong.Split(',');
            var p = s.Count() == 2 ? new Point(Convert.ToDouble(s[0]), Convert.ToDouble(s[1])) : null;
        }
    }
}
