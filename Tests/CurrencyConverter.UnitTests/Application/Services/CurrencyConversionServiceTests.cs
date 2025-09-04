using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.DTOs;
using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Application.Services;
using CurrencyConverter.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CurrencyConverter.Tests.Unit.Application.Services
{
    public class CurrencyConversionServiceTests
    {
        [Fact]
        public async Task ConvertAsync_Uses_Provider_And_Returns_Result()
        {
            // Arrange
            var provider = new Mock<ICurrencyProvider>();
            provider.SetupGet(p => p.Name).Returns("Frankfurter");
            provider.Setup(p => p.GetExchangeRateAsync(
                    It.IsAny<CurrencyCode>(), It.IsAny<CurrencyCode>(), default))
                .ReturnsAsync(1.2m);

            var factoryMock = new Mock<ICurrencyProviderFactory>();
            factoryMock.Setup(f => f.GetProvider(It.IsAny<string>())).Returns(provider.Object);

            var apiServiceMock = new Mock<ICurrencyApiService>();

            var sut = new CurrencyConversionService(
                factoryMock.Object,
                apiServiceMock.Object,
                new[] { "TRY", "PLN", "THB", "MXN" } // blocked currencies
            );

            var dto = new ConversionRequestDto
            {
                From = "EUR",
                To = "USD",
                Amount = 10m,
                Provider = "Frankfurter"
            };

            // Act
            var result = await sut.ConvertAsync(dto);

            // Assert
            result.From.Should().Be("EUR");
            result.To.Should().Be("USD");
            result.ConvertedAmount.Should().Be(12m); // 10 * 1.2
            result.OriginalAmount.Should().Be(10m);
        }

        [Theory]
        [InlineData("TRY")]
        [InlineData("PLN")]
        [InlineData("THB")]
        [InlineData("MXN")]
        public async Task ConvertAsync_Blocks_Excluded_Currencies(string blockedCurrency)
        {
            // Arrange
            var provider = new Mock<ICurrencyProvider>();
            provider.SetupGet(p => p.Name).Returns("DummyProvider");

            var factoryMock = new Mock<ICurrencyProviderFactory>();
            factoryMock.Setup(f => f.GetProvider(It.IsAny<string>())).Returns(provider.Object);

            var apiServiceMock = new Mock<ICurrencyApiService>();

            var sut = new CurrencyConversionService(
                factoryMock.Object,
                apiServiceMock.Object,
                new[] { "TRY", "PLN", "THB", "MXN" } // blocked currencies
            );

            var dto = new ConversionRequestDto
            {
                From = blockedCurrency,  // Blocked currency
                To = "USD",
                Amount = 5m,
                Provider = "Frankfurter"
            };

            // Act
            Func<Task> act = () => sut.ConvertAsync(dto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                     .WithMessage($"{blockedCurrency} is blocked.");
        }
    }
}
