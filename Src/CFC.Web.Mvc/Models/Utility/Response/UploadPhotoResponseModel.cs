using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CFC.Web.Mvc.Models.Vehicle
{
    public class UploadPhotoResponseModel : JsonResponseModel
    {
        public string FileName { get; set; }
    }
}