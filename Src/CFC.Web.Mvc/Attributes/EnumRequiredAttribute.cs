using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CFC.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class EnumRequiredAttribute : RequiredAttribute
    {
        public EnumRequiredAttribute()
        { }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            int parsed = (int)value;

            // Our enums always begin with 1
            return parsed > 0;
        }
    }

}