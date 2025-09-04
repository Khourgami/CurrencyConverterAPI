using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.ValueObjects;
using System.Net.Http.Json;

namespace CurrencyConverter.Infrastructure.Services
{
    public class CurrencyApiService : ICurrencyApiService
    {
        private readonly HttpClient _httpClient;

        public CurrencyApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetExchangeRateAsync(Currency from, Currency to)
        {
            // Example: call a free currency API (exchangerate.host)
            var url = $"https://api.exchangerate.host/convert?from={from.Code}&to={to.Code}";
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);

            if (response == null || response.Result == 0)
                throw new InvalidOperationException($"Failed to fetch exchange rate {from.Code} -> {to.Code}");

            return response.Result;
        }

        public async Task<Dictionary<Currency, decimal>> GetLatestRatesAsync(Currency baseCurrency)
        {
            var url = $"https://api.exchangerate.host/latest?base={baseCurrency.Code}";
            var response = await _httpClient.GetFromJsonAsync<LatestRatesResponse>(url);

            if (response == null || response.Rates == null)
                throw new InvalidOperationException($"Failed to fetch latest rates for {baseCurrency.Code}");

            return response.Rates.ToDictionary(
                kvp => Currency.From(kvp.Key),
                kvp => kvp.Value
            );
        }

        // internal DTOs for JSON deserialization
        private class ExchangeRateResponse
        {
            public decimal Result { get; set; }
        }

        private class LatestRatesResponse
        {
            public string Base { get; set; } = string.Empty;
            public Dictionary<string, decimal> Rates { get; set; } = new();
        }
    }
}
