using CurrencyConverter.Domain.Errors;

namespace CurrencyConverter.Domain.Policies;

/// <summary>
/// Centralized currency rules that don’t depend on infrastructure.
/// </summary>
public static class CurrencyPolicies
{
    /// <summary>
    /// Business rule: these currencies are excluded from requests/responses.
    /// </summary>
    public static readonly HashSet<string> Blacklist =
        new(StringComparer.OrdinalIgnoreCase) { "TRY", "PLN", "THB", "MXN" };

    public static void EnsureNotBlacklisted(string currency)
    {
        if (Blacklist.Contains(currency))
            throw new ValidationException(ErrorCodes.CurrencyBlacklisted,
                $"Currency '{currency}' is not allowed by policy.");
    }
}
