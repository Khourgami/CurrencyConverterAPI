using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.Primitives;
using CurrencyConverter.Domain.ValueObjects;

namespace CurrencyConverter.Domain.Entities;

/// <summary>
/// An exchange rate relative to a base currency at a point in time.
/// For example: Base = EUR, Quote = USD, Rate = 1.08
/// </summary>
public sealed class ExchangeRate : IEquatable<ExchangeRate>
{
    public CurrencyCode BaseCurrency { get; }
    public CurrencyCode QuoteCurrency { get; }
    public decimal Rate { get; } // how many Quote per 1 Base

    private ExchangeRate(CurrencyCode @base, CurrencyCode quote, decimal rate)
    {
        BaseCurrency = @base;
        QuoteCurrency = quote;
        Rate = rate;
    }

    public static ExchangeRate Create(CurrencyCode @base, CurrencyCode quote, decimal rate)
    {
        Guard.AgainstNull(@base, nameof(@base));
        Guard.AgainstNull(quote, nameof(quote));
        Guard.AgainstOutOfRange(rate <= 0m, ErrorCodes.Validation, "Rate must be > 0.");
        return new ExchangeRate(@base, quote, decimal.Round(rate, 9, MidpointRounding.AwayFromZero));
    }

    public bool Equals(ExchangeRate? other) =>
        other is not null &&
        BaseCurrency.Equals(other.BaseCurrency) &&
        QuoteCurrency.Equals(other.QuoteCurrency) &&
        Rate == other.Rate;

    public override bool Equals(object? obj) => obj is ExchangeRate r && Equals(r);
    public override int GetHashCode() => HashCode.Combine(BaseCurrency, QuoteCurrency, Rate);
    public override string ToString() => $"1 {BaseCurrency} = {Rate} {QuoteCurrency}";
}
