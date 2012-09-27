namespace CFC.Web.Mvc.Models.Login
{
    public class LoginResponseModel : JsonResponseModel
    {
        public string Role { get; set; }
        public string AuthToken { get; set; }
        public int ServiceAreaDistance { get; set; }
    }
}