using CurrencyConverter.Api.Models;
using CurrencyConverter.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CurrencyConverter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DebugController> _logger;

        public DebugController(IConfiguration configuration, ILogger<DebugController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // Try to validate token manually (AllowAnonymous so you can call it even when auth breaks)
        [HttpGet("validate-token")]
        [AllowAnonymous]
        public IActionResult ValidateToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Missing Authorization header. Use: Authorization: Bearer <token>");

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Secret"];

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.Name,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // success — show claims the framework sees
                var claims = principal.Claims.Select(c => new { c.Type, c.Value }).ToList();
                return Ok(new
                {
                    Valid = true,
                    Message = "Token validated successfully.",
                    Claims = claims
                });
            }
            catch (SecurityTokenException stEx)
            {
                _logger.LogError(stEx, "Token validation failed: {Message}", stEx.Message);
                return Unauthorized(new { Valid = false, Error = stEx.Message, Type = stEx.GetType().FullName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error validating token");
                return StatusCode(500, new { Valid = false, Error = ex.Message, Type = ex.GetType().FullName });
            }
        }

        [HttpGet("TestAuth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            return Ok("result is okay");
        }

    }
}
