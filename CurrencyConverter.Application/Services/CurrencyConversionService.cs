using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.DTOs;
using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.Services
{
    public sealed class CurrencyConversionService : ICurrencyConversionService
    {
        private readonly ICurrencyProviderFactory _providerFactory;
        private readonly ICurrencyApiService _currencyApiService;
        private readonly string[] _excludedCurrencies;

        public CurrencyConversionService(
            ICurrencyProviderFactory providerFactory,
            ICurrencyApiService currencyApiService,
            string[] excludedCurrencies = null)
        {
            _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
            _currencyApiService = currencyApiService ?? throw new ArgumentNullException(nameof(currencyApiService));
            _excludedCurrencies = excludedCurrencies ?? Array.Empty<string>();
        }

        public async Task<ConversionResultDto> ConvertAsync(ConversionRequestDto request, CancellationToken ct = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            // Blocked source currency check
            if (_excludedCurrencies.Any(c => string.Equals(c, request.From, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"{request.From} is blocked.");

            var provider = _providerFactory.GetProvider(request.From)
                           ?? throw new InvalidOperationException($"No provider found for {request.From}");

            var rate = await provider.GetExchangeRateAsync(
                CurrencyCode.Create(request.From),
                CurrencyCode.Create(request.To),
                ct);

            var convertedAmount = request.Amount * rate;

            return new ConversionResultDto(
                OriginalAmount: request.Amount,
                From: request.From,
                To: request.To,
                ConvertedAmount: convertedAmount,
                RateDate: DateTime.UtcNow);
        }
    }
}
