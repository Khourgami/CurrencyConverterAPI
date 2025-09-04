namespace CurrencyConverter.Application.DTOs;

public sealed record HistoricalRatesRequestDto(
    string BaseCurrency,
    DateOnly StartDate,
    DateOnly EndDate,
    int Page,
    int PageSize,
    string Provider = "Frankfurter");
