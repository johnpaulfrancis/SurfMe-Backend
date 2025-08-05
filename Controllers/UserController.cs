using Microsoft.AspNetCore.Mvc;
using SurfMe.Models.User;

namespace SurfMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Dictionary<string, string> users = new()
        {
            { "admin", "admin123" },
            { "john", "password" }
        };

        [HttpPost("login")]
        public IActionResult UserLogin([FromBody] LoginModel model)
        {
            if (users.TryGetValue(model.UserName, out var storedPassword))
            {
                if (storedPassword == model.Password)
                {
                    return Ok(new { message = "Login successful", ipAddress = model.IpAddress });
                }
            }
            return Unauthorized(new { message = "Invalid username or password" });
        }
    }
}
