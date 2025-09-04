using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.DTOs;
using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.ValueObjects;

namespace CurrencyConverter.Application.Services;

public sealed class ExchangeRateService : IExchangeRateService
{
    private readonly ICurrencyProviderFactory _providerFactory;

    public ExchangeRateService(ICurrencyProviderFactory providerFactory)
    {
        _providerFactory = providerFactory;
    }

    public async Task<ExchangeRateDto> GetLatestRatesAsync(string baseCurrency, CancellationToken ct = default)
    {
        var provider = _providerFactory.GetProvider("Frankfurter"); // default for now
        var baseCcy = CurrencyCode.Create(baseCurrency, enforceBlacklist: true);

        var snapshot = await provider.GetLatestRatesAsync(baseCcy, ct);

        return new ExchangeRateDto(
            snapshot.BaseCurrency.Value,
            new DateTime(snapshot.Date.Year, snapshot.Date.Month, snapshot.Date.Day),
            snapshot.Rates);
    }

    public async Task<HistoricalRatesResultDto> GetHistoricalRatesAsync(
        HistoricalRatesRequestDto request,
        CancellationToken ct = default)
    {
        var provider = _providerFactory.GetProvider(request.Provider);
        var baseCcy = CurrencyCode.Create(request.BaseCurrency, enforceBlacklist: true);
        var range = DateRange.Create(request.StartDate, request.EndDate);

        var snapshots = await provider.GetHistoricalRatesAsync(baseCcy, range, ct);

        var total = snapshots.Count;
        var items = snapshots
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ExchangeRateDto(
                s.BaseCurrency.Value,
                new DateTime(s.Date.Year, s.Date.Month, s.Date.Day),
                s.Rates))
            .ToList();

        return new HistoricalRatesResultDto(
            baseCcy.Value,
            items,
            request.Page,
            request.PageSize,
            total);
    }
}
