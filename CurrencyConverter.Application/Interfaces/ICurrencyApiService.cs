using CurrencyConverter.Domain.ValueObjects;

public interface ICurrencyApiService
{
    Task<decimal> GetExchangeRateAsync(Currency from, Currency to);
    Task<Dictionary<Currency, decimal>> GetLatestRatesAsync(Currency baseCurrency);
}
