using Asp.Versioning;
using CurrencyConverter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CurrencyConverter.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly List<UserConfig> _users;

        public AuthController(ITokenService tokenService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _configuration = configuration;

            // Read users from config
            _users = _configuration.GetSection("Users")
                                   .Get<List<UserConfig>>() ?? new List<UserConfig>();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

            var user = _users.FirstOrDefault(u =>
                u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == request.Password);

            if (user == null)
                return Unauthorized();

            // Generate JWT token with role from config
            var token = _tokenService.GenerateToken(user.Username, user.Role);

            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class UserConfig
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";
    }
}
