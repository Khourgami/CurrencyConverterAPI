using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.Primitives;

namespace CurrencyConverter.Domain.ValueObjects;

/// <summary>
/// Monetary amount in a specific currency. Amount stored as decimal for safety.
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public CurrencyCode Currency { get; }

    private Money(decimal amount, CurrencyCode currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, CurrencyCode currency)
    {
        Guard.AgainstNull(currency, nameof(currency));
        return new Money(amount, currency);
    }

    public Money WithAmount(decimal newAmount) => new(newAmount, Currency);

    public bool Equals(Money? other) =>
        other is not null && Amount == other.Amount && Currency.Equals(other.Currency);

    public override bool Equals(object? obj) => obj is Money m && Equals(m);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount} {Currency}";
}
