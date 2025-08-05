namespace SurfMe.Models.User
{
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}
