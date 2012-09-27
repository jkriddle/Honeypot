using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CFC.Domain;

namespace CFC.Web.Mvc.Models.User
{
    public class EditProfileInputModel : BaseInputModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string Password { get; set; }
        public string CellPhone{ get; set; }
        public int? ServiceAreaDistance { get; set; }

        [DisplayName("Confirm Password"), Compare("Password")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Bid Threshold")]
        public decimal? BidThreshold { get; set; }

        /// <summary>
        /// File name of photo uploaded for this vehicle
        /// </summary>
        public string UserPhotoName { get; set; }
    }
}