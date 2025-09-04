using System;
using System.Threading;
using System.Threading.Tasks;
using CurrencyConverter.Api.Controllers;
using CurrencyConverter.Api.Models;
using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CurrencyConverter.Tests.Unit.Api.Controllers
{
    public class CurrencyControllerTests
    {
        [Fact]
        public async Task Convert_Should_Return_BadRequest_When_Request_Is_Null()
        {
            // Arrange
            var svc = new Mock<ICurrencyConversionService>();
            var controller = new CurrencyController(svc.Object);

            // Act
            var result = await controller.Convert(null!, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be("Request cannot be null.");
        }

        [Fact]
        public async Task Convert_Should_Return_BadRequest_When_Currencies_Are_Missing()
        {
            // Arrange
            var svc = new Mock<ICurrencyConversionService>();
            var controller = new CurrencyController(svc.Object);

            var req = new CurrencyConversionRequest
            {
                From = "",
                To = "EUR",
                Amount = 10m
            };

            // Act
            var result = await controller.Convert(req, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be("Source and target currencies must be provided.");
        }

        [Fact]
        public async Task Convert_Should_Return_BadRequest_When_Amount_Is_Not_Positive()
        {
            // Arrange
            var svc = new Mock<ICurrencyConversionService>();
            var controller = new CurrencyController(svc.Object);

            var req = new CurrencyConversionRequest
            {
                From = "USD",
                To = "EUR",
                Amount = 0m
            };

            // Act
            var result = await controller.Convert(req, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                  .Which.Value.Should().Be("Amount must be greater than zero.");
        }

        [Fact]
        public async Task Convert_Should_Return_Ok_With_ConversionResultDto_When_Request_Is_Valid()
        {
            // Arrange
            var svc = new Mock<ICurrencyConversionService>();
            var controller = new CurrencyController(svc.Object);

            var req = new CurrencyConversionRequest
            {
                From = "USD",
                To = "EUR",
                Amount = 100m
            };

            var now = DateTime.UtcNow;

            // IMPORTANT: Use the exact DTO constructor shape (5 args)
            var expected = new ConversionResultDto(
                OriginalAmount: 100m,
                From: "USD",
                To: "EUR",
                ConvertedAmount: 92.34m,
                RateDate: now
            );

            svc.Setup(s => s.ConvertAsync(
                    It.IsAny<ConversionRequestDto>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expected);

            // Act
            var result = await controller.Convert(req, CancellationToken.None);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeOfType<ConversionResultDto>().Subject;

            dto.OriginalAmount.Should().Be(100m);
            dto.From.Should().Be("USD");
            dto.To.Should().Be("EUR");
            dto.ConvertedAmount.Should().Be(92.34m);
            dto.RateDate.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
        }
    }
}
