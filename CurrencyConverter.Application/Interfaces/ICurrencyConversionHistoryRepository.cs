using CurrencyConverter.Domain.Entities;

namespace CurrencyConverter.Application.Interfaces;

public interface ICurrencyConversionHistoryRepository
{
    Task AddAsync(CurrencyConversionHistory history);
    Task<IEnumerable<CurrencyConversionHistory>> GetAllAsync();
}
