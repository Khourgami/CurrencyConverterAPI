namespace CurrencyConverter.Domain.ValueObjects;

/// <summary>
/// Thin wrapper around CurrencyCode for backward compatibility with code/tests
/// that expect a 'Currency' value object. All validation is delegated to CurrencyCode.
/// </summary>
public sealed class Currency : IEquatable<Currency>
{
    /// <summary> Three-letter ISO code (always uppercase). </summary>
    public string Code { get; }

    private Currency(string code) => Code = code;

    /// <summary>
    /// Creates a Currency after validating via CurrencyCode (length, letters, blacklist).
    /// </summary>
    public static Currency Create(string code, bool enforceBlacklist = true)
    {
        var cc = CurrencyCode.Create(code, enforceBlacklist);
        return new Currency(cc.Value);
    }

    /// <summary> Convenience alias. </summary>
    public static Currency From(string code, bool enforceBlacklist = true) =>
        Create(code, enforceBlacklist);

    /// <summary> Convert to the canonical CurrencyCode VO. </summary>
    public CurrencyCode ToCurrencyCode() => CurrencyCode.Create(Code);

    /// <summary> Implicit cast to CurrencyCode for ergonomic interop in existing code. </summary>
    public static implicit operator CurrencyCode(Currency currency) =>
        CurrencyCode.Create(currency.Code);

    public bool Equals(Currency? other) =>
        other is not null && string.Equals(Code, other.Code, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is Currency c && Equals(c);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Code);

    public override string ToString() => Code;
}
