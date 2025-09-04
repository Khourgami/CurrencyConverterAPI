using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.ValueObjects;
using FluentAssertions;
using System;
using Xunit;

namespace CurrencyConverter.Tests.Unit.Domain.ValueObjects
{
    public class CurrencyCodeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("12$")]
        [InlineData("US")]
        [InlineData("USDD")]
        public void Create_Throws_On_Invalid(string input)
        {
            // Act
            Action act = () => CurrencyCode.Create(input);

            // Assert
            act.Should().Throw<ValidationException>();
        }

        [Theory]
        [InlineData("USD")]
        [InlineData("EUR")]
        [InlineData("JPY")]
        public void Create_Succeeds_On_Valid(string input)
        {
            // Act
            var currencyCode = CurrencyCode.Create(input);

            // Assert
            currencyCode.Value.Should().Be(input);
        }
    }
}
