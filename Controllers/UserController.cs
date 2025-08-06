using Microsoft.AspNetCore.Mvc;
using SurfMe.Models.User;
using SurfMe.Service;

namespace SurfMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JWTService _jwtService;

        public UserController(JWTService jwtService)
        {
            _jwtService = jwtService;
        }

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
                    var token = _jwtService.GenerateToken(model.UserName);
                    return Ok(new { token });
                }
            }
            return Unauthorized(new { message = "Invalid username or password" });
        }
    }
}
