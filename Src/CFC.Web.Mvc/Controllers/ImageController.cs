using System;
using System.IO;
using System.Web.Helpers;
using CFC.Services.UserService;
using CFC.Web.Mvc.Attributes;
using CFC.Web.Mvc.Helpers;
using CFC.Web.Mvc.Models.Vehicle;

namespace CFC.Web.Mvc.Controllers
{

    public class ImageController : BaseController
    {
        private readonly IUserService _userService;


        public ImageController()
           
        {
          
        }

        /// <summary>
        /// Display an image for a specified object
        /// </summary>
        /// <param name="id">Object ID</param>
        /// <param name="type">Object type to retrieve image for</param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        public void View(int id, string type, int maxWidth = 280, int maxHeight = 280)
        {
            var imageBytes = new byte[0];
            string defaultImage;

            switch(type.ToLower())
            {
                case "user":
                    var user = _userService.GetUserById(id);
                    if (user != null) imageBytes = user.Image;
                    defaultImage = "~/Content/img/default_user.png";
                    break;
                default:
                    throw new Exception("Invalid image type specified in type parameter.");
            }

            WebImage image = null;

            if (imageBytes == null || imageBytes.Length == 0)
            {
                image = new WebImage(new FileStream(Request.MapPath(defaultImage), FileMode.Open));
            }
            else
            {
                image = new WebImage(imageBytes);
            }

            image.Resize(maxWidth, maxHeight);
            image.Write();
        }

        /// <summary>
        /// Upload an image (using the jQuery FileUplader control. 
        /// @see https://github.com/valums/file-uploader
        /// </summary>
        [RequiresAuthentication]
        public void Upload()
        {
            // Set the response return data type
            Response.ContentType = "application/json";
            var vm = new UploadPhotoResponseModel();

            try
            {
                // First check this header (cross browser support)
                string uploadFileName = Request.Headers["X-File-Name"];

                if (string.IsNullOrEmpty(uploadFileName) == false || Request.Files.Count > 0)
                {
                    // Get the uploads physical directory on the server
                    string directory = Server.MapPath(ConfigHelper.AppSetting("FileUploadPath"));

                    // get just the original filename
                    string filename = System.Guid.NewGuid().ToString() + ".jpg";

                    // create full server path
                    string file = string.Format("{0}\\{1}", directory, filename);

                    // If file exists already, delete it (optional)
                    if (System.IO.File.Exists(file) == true) System.IO.File.Delete(file);

                    if (string.IsNullOrEmpty(uploadFileName) == true) // IE Browsers
                    {
                        // Save file to server
                        Request.Files[0].SaveAs(file);
                    }
                    else // Other Browsers
                    {
                        // Save file to server
                        using (var fileStream = new System.IO.FileStream(file, System.IO.FileMode.OpenOrCreate))
                        {
                            Request.InputStream.CopyTo(fileStream);
                            fileStream.Close();
                        }
                    }

                    vm.FileName = filename;
                    vm.Success = true;
                }
            }
            catch (Exception)
            {
                vm.Success = false;
            }

            Response.Write(JsonHelper.ConvertObject(vm));
            Response.End();
        }


    }
}
