using CurrencyConverter.Application.DTOs;

namespace CurrencyConverter.Application.Abstractions;

public interface IExchangeRateService
{
    Task<ExchangeRateDto> GetLatestRatesAsync(string baseCurrency, CancellationToken ct = default);

    Task<HistoricalRatesResultDto> GetHistoricalRatesAsync(
        HistoricalRatesRequestDto request,
        CancellationToken ct = default);
}
