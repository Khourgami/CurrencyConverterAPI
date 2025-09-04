namespace CurrencyConverter.Application.DTOs;

public sealed record HistoricalRatesResultDto(
    string BaseCurrency,
    IEnumerable<ExchangeRateDto> Items,
    int Page,
    int PageSize,
    int TotalCount);
