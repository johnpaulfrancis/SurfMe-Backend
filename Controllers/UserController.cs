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
        private readonly EncryptDecryptService _encryptDecryptService;

        public UserController(JWTService jwtService, ApplicationDbContext applicationDbContext, EncryptDecryptService encryptDecryptService)
        {
            _jwtService = jwtService;
            _applicationDbContext = applicationDbContext;
            _encryptDecryptService = encryptDecryptService;
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(LoginModel model)
        {
            var user = await _applicationDbContext.Tbl_Users
                .FirstOrDefaultAsync(u => u.LoginName == model.UserName.Trim() && u.Password == model.Password.Trim() && u.IsActive == true);
            if (user != null)
            {
                var token = _jwtService.GenerateToken(user.Name, _encryptDecryptService.Encrypt(Convert.ToString(user.UserId)));
                return Ok(new { statusCode = 200, token });
            }
            return Unauthorized(new { statusCode = 401, message = "Invalid username or password" });
        }
    }
}
