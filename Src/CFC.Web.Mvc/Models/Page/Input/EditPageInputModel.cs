using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CFC.Web.Mvc.Models.Page
{
    public class EditPageInputModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}