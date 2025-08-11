using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfMe.Data;
using SurfMe.Models.User;
using SurfMe.Service;

namespace SurfMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly ApplicationDbContext _applicationDbContext;

        public UserController(JWTService jwtService, ApplicationDbContext applicationDbContext)
        {
            _jwtService = jwtService;
            _applicationDbContext = applicationDbContext;
        }


        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(LoginModel model)
        {
            var user = await _applicationDbContext.Tbl_Users
                .FirstOrDefaultAsync(u => u.LoginName == model.UserName && u.Password == model.Password && u.IsActive);
            if (user != null)
            {
                var token = _jwtService.GenerateToken(model.UserName);
                return Ok(new { statusCode = 200, token });
            }
            return Unauthorized(new { statusCode = 401, message = "Invalid username or password" });
        }
    }
}
