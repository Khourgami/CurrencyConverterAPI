using Asp.Versioning;
using CurrencyConverter.Api.Models;
using CurrencyConverter.Application.Abstractions;
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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConversionService _currencyConversionService;

        public CurrencyController(ICurrencyConversionService currencyConversionService)
        {
            _currencyConversionService = currencyConversionService;
        }

        /// <summary>
        /// Converts an amount from one currency to another
        /// </summary>
        /// <param name="request">Currency conversion request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Conversion result</returns>
        [HttpPost("convert")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Convert([FromBody] CurrencyConversionRequest request, CancellationToken ct)
        {
            if (request == null)
                return BadRequest("Request cannot be null.");

            if (string.IsNullOrWhiteSpace(request.From) || string.IsNullOrWhiteSpace(request.To))
                return BadRequest("Source and target currencies must be provided.");

            if (request.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            // Map API request to Application DTO
            var requestDto = new ConversionRequestDto
            {
                From = request.From,
                To = request.To,
                Amount = request.Amount
            };

            // Call Application service
            var result = await _currencyConversionService.ConvertAsync(requestDto, ct);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            var user = HttpContext.User;

            // See if authentication worked
            var isAuth = user.Identity?.IsAuthenticated ?? false;

            // Check claims
            var claims = user.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();

            // Check roles
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                IsAuthenticated = isAuth,
                AuthenticationType = user.Identity?.AuthenticationType,
                Claims = claims,
                Roles = roles
            });
        }


        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult Health() => Ok(new { Status = "Healthy" });

    }
}
