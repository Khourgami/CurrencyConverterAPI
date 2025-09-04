using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.Primitives;
using CurrencyConverter.Domain.ValueObjects;

namespace CurrencyConverter.Domain.Entities;

/// <summary>
/// Immutable snapshot of rates for a specific base currency and date.
/// Rates dictionary maps QUOTE currency -> rate (quote per 1 base).
/// </summary>
public sealed class RateSnapshot : IEquatable<RateSnapshot>
{
    public DateOnly Date { get; }
    public CurrencyCode BaseCurrency { get; }
    public IReadOnlyDictionary<string, decimal> Rates { get; }

    private RateSnapshot(DateOnly date, CurrencyCode baseCurrency, IReadOnlyDictionary<string, decimal> rates)
    {
        Date = date;
        BaseCurrency = baseCurrency;
        Rates = rates;
    }

    public static RateSnapshot Create(DateOnly date, CurrencyCode baseCurrency, IDictionary<string, decimal> rates)
    {
        Guard.AgainstNull(baseCurrency, nameof(baseCurrency));
        Guard.AgainstNull(rates, nameof(rates));

        // Normalize: uppercase keys, positive rates only
        var normalized = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in rates)
        {
            if (string.IsNullOrWhiteSpace(kv.Key)) continue;
            if (kv.Value <= 0m) continue;
            normalized[kv.Key.ToUpperInvariant()] = decimal.Round(kv.Value, 9, MidpointRounding.AwayFromZero);
        }

        if (normalized.Count == 0)
            throw new ValidationException(ErrorCodes.RateNotFound, "No valid rates provided.");

        return new RateSnapshot(date, baseCurrency,
            new Dictionary<string, decimal>(normalized, StringComparer.OrdinalIgnoreCase));
    }

    public decimal GetDirectRate(CurrencyCode target)
    {
        if (target.Equals(BaseCurrency)) return 1m;

        if (!Rates.TryGetValue(target.Value, out var rate))
            throw new ValidationException(ErrorCodes.RateNotFound,
                $"Rate for {BaseCurrency}->{target} not found in snapshot {Date:yyyy-MM-dd}.");

        return rate;
    }

    public bool Equals(RateSnapshot? other)
    {
        if (other is null) return false;
        if (Date != other.Date) return false;
        if (!BaseCurrency.Equals(other.BaseCurrency)) return false;
        if (Rates.Count != other.Rates.Count) return false;

        foreach (var kv in Rates)
        {
            if (!other.Rates.TryGetValue(kv.Key, out var v)) return false;
            if (v != kv.Value) return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => obj is RateSnapshot r && Equals(r);
    public override int GetHashCode() => HashCode.Combine(Date, BaseCurrency, Rates.Count);
    public override string ToString() => $"{Date:yyyy-MM-dd} [{BaseCurrency}] rates={Rates.Count}";
}
