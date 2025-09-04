using CurrencyConverter.Application.DTOs;

namespace CurrencyConverter.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string role);
    }
}
