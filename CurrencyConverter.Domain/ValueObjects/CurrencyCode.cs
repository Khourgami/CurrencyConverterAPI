using System.Text.RegularExpressions;
using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.Policies;
using CurrencyConverter.Domain.Primitives;

namespace CurrencyConverter.Domain.ValueObjects;

/// <summary>
/// ISO-4217 currency code (3 letters), case-insensitive, stored uppercase.
/// </summary>
public sealed class CurrencyCode : IEquatable<CurrencyCode>
{
    private static readonly Regex Iso4217 = new("^[A-Za-z]{3}$", RegexOptions.Compiled);

    public string Value { get; }

    private CurrencyCode(string value) => Value = value;

    public static CurrencyCode Create(string code, bool enforceBlacklist = false)
    {
        Guard.AgainstNullOrWhiteSpace(code, nameof(code));
        if (!Iso4217.IsMatch(code))
            throw new ValidationException(ErrorCodes.CurrencyNotSupported,
                $"'{code}' is not a valid ISO-4217 currency code.");

        var normalized = code.ToUpperInvariant();

        if (enforceBlacklist)
            CurrencyPolicies.EnsureNotBlacklisted(normalized);

        return new CurrencyCode(normalized);
    }

    public override string ToString() => Value;

    public bool Equals(CurrencyCode? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is CurrencyCode c && Equals(c);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
}
