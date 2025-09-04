using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Application.Services;
using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace CurrencyConverter.Tests.Unit.Domain.ValueObjects
{
    public class DateRangeTests
    {
        [Fact]
        public void Create_Valid_Range()
        {
            var start = new DateOnly(2024, 1, 1);
            var end = new DateOnly(2024, 1, 31);

            var range = DateRange.Create(start, end);

            range.Start.Should().Be(start);
            range.End.Should().Be(end);
        }

        [Fact]
        public void Create_Throws_When_End_Before_Start()
        {
            // Arrange
            var start = new DateOnly(2025, 1, 10);
            var end = new DateOnly(2025, 1, 5);

            // Act & Assert
            var ex = Assert.Throws<ValidationException>(() => DateRange.Create(start, end));

            Assert.Equal("DateRange.InvalidPeriod", ex.Code);
            Assert.Contains("cannot be before", ex.Message);
        }


        [Fact]
        public void Contains_Works()
        {
            var range = DateRange.Create(new DateOnly(2024, 1, 1), new DateOnly(2024, 1, 10));
            range.Contains(new DateOnly(2024, 1, 5)).Should().BeTrue();
            range.Contains(new DateOnly(2024, 1, 11)).Should().BeFalse();


        }
    }
}
