using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Domain.ValueObjects;

namespace CurrencyConverter.Application.Interfaces;

/// <summary>
/// Contract for any external currency data provider (Frankfurter, Fixer, etc.)
/// </summary>
public interface ICurrencyProvider
{
    string Name { get; }

    Task<RateSnapshot> GetLatestRatesAsync(CurrencyCode baseCurrency, CancellationToken ct = default);

    Task<decimal> GetExchangeRateAsync(CurrencyCode fromCurrency, CurrencyCode toCurrency, CancellationToken ct = default);


    Task<IReadOnlyList<RateSnapshot>> GetHistoricalRatesAsync(
        CurrencyCode baseCurrency,
        DateRange range,
        CancellationToken ct = default);
}
