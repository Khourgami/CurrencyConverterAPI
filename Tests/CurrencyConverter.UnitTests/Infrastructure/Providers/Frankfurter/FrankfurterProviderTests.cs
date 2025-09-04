using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Domain.ValueObjects;
using CurrencyConverter.Infrastructure.Providers.Frankfurter;
using CurrencyConverter.Tests.Unit.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Xunit;

namespace CurrencyConverter.Tests.Unit.Infrastructure.Providers.Frankfurter
{
    public class FrankfurterProviderTests
    {
        private static IMemoryCache NewMemoryCache() => new MemoryCache(new MemoryCacheOptions());

        [Fact]
        public async Task GetExchangeRate_Uses_Http_Then_Cache()
        {
            // Arrange response JSON matching Frankfurter /latest
            var json = """
            {
              "base": "EUR",
              "date": "2025-08-22",
              "rates": { "USD": 1.11 }
            }
            """;

            var handler = new FakeHttpMessageHandler(_ => FakeHttpMessageHandler.Json(json));
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.frankfurter.app/") };

            var httpFactory = new Mock<IHttpClientFactory>();
            httpFactory
                .Setup(f => f.CreateClient(FrankfurterProvider.HttpClientName))
                .Returns(httpClient);

            var cache = NewMemoryCache();
            var options = Options.Create(new FrankfurterOptions { BaseUrl = "https://api.frankfurter.app/" });
            var logger = NullLogger<FrankfurterProvider>.Instance;

            var sut = new FrankfurterProvider(httpFactory.Object, cache, options, logger);

            // Act (first call hits HTTP)
            var rate1 = await sut.GetExchangeRateAsync(CurrencyCode.Create("EUR"), CurrencyCode.Create("USD"));
            // Act (second call hits cache)
            var rate2 = await sut.GetExchangeRateAsync(CurrencyCode.Create("EUR"), CurrencyCode.Create("USD"));

            // Assert
            rate1.Should().Be(1.11m);
            rate2.Should().Be(1.11m);
            handler.CallCount.Should().Be(1, "second call should be served from cache");
        }

        [Fact]
        public async Task GetLatestRates_Returns_Snapshot()
        {
            var json = """
            {
              "base": "EUR",
              "date": "2025-08-22",
              "rates": { "USD": 1.11, "GBP": 0.86 }
            }
            """;

            var handler = new FakeHttpMessageHandler(_ => FakeHttpMessageHandler.Json(json));
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.frankfurter.app/") };

            var httpFactory = new Mock<IHttpClientFactory>();
            httpFactory.Setup(f => f.CreateClient(FrankfurterProvider.HttpClientName)).Returns(httpClient);

            var cache = new MemoryCache(new MemoryCacheOptions());
            var options = Options.Create(new FrankfurterOptions { BaseUrl = "https://api.frankfurter.app/" });
            var logger = NullLogger<FrankfurterProvider>.Instance;

            var sut = new FrankfurterProvider(httpFactory.Object, cache, options, logger);

            var snap = await sut.GetLatestRatesAsync(CurrencyCode.Create("EUR"));

            snap.BaseCurrency.Value.Should().Be("EUR");
            snap.Rates.Should().ContainKey("USD");
            handler.CallCount.Should().Be(1);
        }

        [Fact]
        public async Task Throws_On_Missing_Target()
        {
            var json = """
            { "base": "EUR", "date": "2025-08-22", "rates": { "GBP": 0.86 } }
            """;

            var handler = new FakeHttpMessageHandler(_ => FakeHttpMessageHandler.Json(json));
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.frankfurter.app/") };
            var httpFactory = new Mock<IHttpClientFactory>();
            httpFactory.Setup(f => f.CreateClient(FrankfurterProvider.HttpClientName)).Returns(httpClient);

            var cache = new MemoryCache(new MemoryCacheOptions());
            var options = Options.Create(new FrankfurterOptions { BaseUrl = "https://api.frankfurter.app/" });
            var logger = NullLogger<FrankfurterProvider>.Instance;

            var sut = new FrankfurterProvider(httpFactory.Object, cache, options, logger);

            var act = () => sut.GetExchangeRateAsync(CurrencyCode.Create("EUR"), CurrencyCode.Create("USD"));
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
