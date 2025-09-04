namespace CurrencyConverter.Application.DTOs;

public sealed record ExchangeRateDto(
    string BaseCurrency,
    DateTime Date,
    IReadOnlyDictionary<string, decimal> Rates);
