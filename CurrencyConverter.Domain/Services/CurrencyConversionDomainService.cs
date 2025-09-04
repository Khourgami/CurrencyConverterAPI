using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.Policies;
using CurrencyConverter.Domain.ValueObjects;

namespace CurrencyConverter.Domain.Services;
using Entities;

/// <summary>
/// Pure domain service for converting money using a rate snapshot.
/// Keeps all math and rules in the domain.
/// </summary>
public static class CurrencyConversionDomainService
{
    /// <summary>
    /// Converts a monetary amount using a given rate snapshot.
    /// Supports Base->X, X->Base, and X->Y (cross) via base.
    /// Enforces currency blacklist as a domain rule.
    /// </summary>
    public static Money Convert(Money source, CurrencyCode targetCurrency, RateSnapshot snapshot)
    {
        // Enforce blacklist per business rules
        CurrencyPolicies.EnsureNotBlacklisted(source.Currency.Value);
        CurrencyPolicies.EnsureNotBlacklisted(targetCurrency.Value);
        CurrencyPolicies.EnsureNotBlacklisted(snapshot.BaseCurrency.Value);

        // If currencies are same, no-op
        if (source.Currency.Equals(targetCurrency))
            return source;

        // Ensure snapshot base is present
        var baseCcy = snapshot.BaseCurrency;

        decimal resultAmount;

        if (source.Currency.Equals(baseCcy))
        {
            // Base -> Target
            var r = snapshot.GetDirectRate(targetCurrency);
            resultAmount = source.Amount * r;
        }
        else if (targetCurrency.Equals(baseCcy))
        {
            // Source -> Base
            var rFrom = snapshot.GetDirectRate(source.Currency);
            resultAmount = source.Amount / rFrom;
        }
        else
        {
            // Cross via base: Source -> Base -> Target
            var rFrom = snapshot.GetDirectRate(source.Currency);
            var rTo = snapshot.GetDirectRate(targetCurrency);
            // amountInBase = amount / rFrom; then * rTo
            var amountInBase = source.Amount / rFrom;
            resultAmount = amountInBase * rTo;
        }

        // Round final to 4 fractional digits (typical for FX quotes). Adjust if needed.
        var rounded = decimal.Round(resultAmount, 4, MidpointRounding.AwayFromZero);
        return Money.Create(rounded, targetCurrency);
    }
}
