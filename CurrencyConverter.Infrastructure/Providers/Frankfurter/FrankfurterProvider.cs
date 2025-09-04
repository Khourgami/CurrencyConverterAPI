using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;

namespace CurrencyConverter.Infrastructure.Providers.Frankfurter;

public sealed class FrankfurterProvider : ICurrencyProvider
{
    public const string HttpClientName = "Frankfurter";
    public string Name => "Frankfurter";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly FrankfurterOptions _options;
    private readonly ILogger<FrankfurterProvider> _logger;

    public FrankfurterProvider(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        IOptions<FrankfurterOptions> options,
        ILogger<FrankfurterProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<decimal> GetExchangeRateAsync(
        CurrencyCode fromCurrency,
        CurrencyCode toCurrency,
        CancellationToken ct = default)
    {
        var cacheKey = $"Frankfurter:{fromCurrency}->{toCurrency}";
        if (_cache.TryGetValue(cacheKey, out decimal cachedRate))
        {
            _logger.LogInformation("Cache hit for {From} -> {To}: {Rate}", fromCurrency, toCurrency, cachedRate);
            return cachedRate;
        }

        var url = $"latest?base={fromCurrency.Value}&symbols={toCurrency.Value}";
        var resp = await GetFromFrankfurterAsync<LatestResponse>(url, ct);

        if (!resp.Rates.TryGetValue(toCurrency.Value, out var rate))
            throw new InvalidOperationException($"Exchange rate not found for {fromCurrency} -> {toCurrency}");

        _cache.Set(cacheKey, rate, TimeSpan.FromMinutes(10));
        _logger.LogInformation("Fetched {From} -> {To}: {Rate}", fromCurrency, toCurrency, rate);

        return rate;
    }

    public async Task<RateSnapshot> GetLatestRatesAsync(
        CurrencyCode baseCurrency,
        CancellationToken ct = default)
    {
        var cacheKey = $"frankfurter:latest:{baseCurrency.Value}";
        if (_cache.TryGetValue(cacheKey, out RateSnapshot cached))
            return cached;

        var url = $"latest?base={baseCurrency.Value}";
        var resp = await GetFromFrankfurterAsync<LatestResponse>(url, ct);

        var date = DateOnly.Parse(resp.Date);
        var snapshot = RateSnapshot.Create(date, CurrencyCode.Create(resp.Base), resp.Rates);
        _cache.Set(cacheKey, snapshot, TimeSpan.FromMinutes(10));

        return snapshot;
    }

    public async Task<IReadOnlyList<RateSnapshot>> GetHistoricalRatesAsync(
        CurrencyCode baseCurrency,
        DateRange range,
        CancellationToken ct = default)
    {
        var url = $"{range.Start:yyyy-MM-dd}..{range.End:yyyy-MM-dd}?base={baseCurrency.Value}";
        var resp = await GetFromFrankfurterAsync<HistoricalResponse>(url, ct);

        var list = resp.RatesByDate.Select(kv =>
        {
            var date = DateOnly.Parse(kv.Key);
            return RateSnapshot.Create(date, CurrencyCode.Create(resp.Base), kv.Value);
        }).OrderBy(r => r.Date).ToList();

        return list;
    }

    // -----------------------------
    // Private helper for API calls
    // -----------------------------
    private async Task<T> GetFromFrankfurterAsync<T>(string url, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);
        var sw = Stopwatch.StartNew();

        try
        {
            var resp = await client.GetFromJsonAsync<T>(url, ct)
                       ?? throw new InvalidOperationException("Empty response from Frankfurter API.");

            sw.Stop();
            _logger.LogInformation("Frankfurter API call {@Url} returned in {@ElapsedMilliseconds}ms", url, sw.ElapsedMilliseconds);

            return resp;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error during Frankfurter API call {@Url} after {@ElapsedMilliseconds}ms", url, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
