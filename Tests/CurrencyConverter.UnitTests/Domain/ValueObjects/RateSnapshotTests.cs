using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CurrencyConverter.Tests.Unit.Domain.Entities
{
    public class RateSnapshotTests
    {
        [Fact]
        public void Create_Stores_Data_Correctly()
        {
            var date = new DateOnly(2024, 1, 1);
            var baseCur = CurrencyCode.Create("EUR");
            var rates = new Dictionary<string, decimal>
            {
                ["USD"] = 1.1m,
                ["GBP"] = 0.86m
            };

            var snap = RateSnapshot.Create(date, baseCur, rates);
            snap.Date.Should().Be(date);
            snap.BaseCurrency.Should().Be(baseCur);
            snap.Rates.Should().ContainKey("USD").WhoseValue.Should().Be(1.1m);
        }
    }
}
