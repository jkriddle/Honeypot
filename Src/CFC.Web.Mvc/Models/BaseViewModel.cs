using System.Collections.Generic;
using CFC.Domain;
using CFC.Services.Validation;

namespace CFC.Web.Mvc.Models
{
    public class BaseViewModel
    {
        public int CurrentUserId { get; set; }
        public Role CurrentRole { get; set; }
        public int CurrentCompanyId { get; set; }
        public string Location { get; set; }
    }
}