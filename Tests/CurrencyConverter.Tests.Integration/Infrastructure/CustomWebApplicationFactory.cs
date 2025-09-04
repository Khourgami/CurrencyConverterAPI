using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.DTOs;
using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.ValueObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace CurrencyConverter.Tests.Integration.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            // Force a predictable environment name
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                // Load our test settings AFTER the app's defaults so they win
                configBuilder.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: false);
            });

            builder.ConfigureServices(services =>
            {
                // Optionally swap any external dependencies that would hit network.
                // Example: replace ICurrencyConversionService with a deterministic fake.
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(ICurrencyConversionService));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton<ICurrencyConversionService, FakeCurrencyConversionService>();
            });

            return base.CreateHost(builder);
        }
    }

    /// <summary>
    /// Deterministic fake for integration tests (no network).
    /// It multiplies amount by 1.20 and returns "now" for RateDate.
    /// </summary>
    internal sealed class FakeCurrencyConversionService : ICurrencyConversionService
    {
        public Task<decimal> ConvertCurrencyAsync(Currency from, Currency to, decimal amount)
        {
            // not used in controller in your current flow, but keep it consistent
            return Task.FromResult(amount * 1.2m);
        }

        public Task<ConversionResultDto> ConvertAsync(ConversionRequestDto request, CancellationToken ct = default)
        {
            var converted = request.Amount * 1.2m;
            var dto = new ConversionResultDto(
                OriginalAmount: request.Amount,
                From: request.From,
                To: request.To,
                ConvertedAmount: converted,
                RateDate: DateTime.UtcNow
            );

            return Task.FromResult(dto);
        }
    }
}
