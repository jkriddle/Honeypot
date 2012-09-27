using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CFC.Services.PagedList;

namespace CFC.Web.Mvc.Models
{
    public class PagedInputModel
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int NumPerPage { get; set; }

        public PagedInputModel()
        {
            CurrentPage = 1;
            NumPerPage = 20;
        }
    }
}