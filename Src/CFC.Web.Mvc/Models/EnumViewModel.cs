using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CFC.Domain;

namespace CFC.Web.Mvc.Models
{
    public class EnumViewModel
    {
        public string Description { get; set; }
        public int Value { get; set; }

        public EnumViewModel(Enum ob)
        {
            Description = ob.GetDescription();
            Value = Convert.ToInt32(ob);
        }
    }
}