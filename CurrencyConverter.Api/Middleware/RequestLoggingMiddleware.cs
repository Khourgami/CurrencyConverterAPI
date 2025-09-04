using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;


namespace CurrencyConverter.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger = Log.ForContext<RequestLoggingMiddleware>();

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var method = context.Request.Method;
            var path = context.Request.Path;

            string? clientId = null;
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..].Trim();
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                clientId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            }

            await _next(context);
            sw.Stop();

            var statusCode = context.Response.StatusCode;
            _logger.Information(
                "Request {@Method} {@Path} from {@ClientIp} by {@ClientId} returned {@StatusCode} in {@ElapsedMilliseconds}ms",
                method, path, clientIp, clientId, statusCode, sw.ElapsedMilliseconds);
        }
    }
}
