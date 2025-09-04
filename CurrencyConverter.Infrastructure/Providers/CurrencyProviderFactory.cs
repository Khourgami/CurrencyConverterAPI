using CurrencyConverter.Application.Interfaces;

namespace CurrencyConverter.Infrastructure.Providers;

public class CurrencyProviderFactory : ICurrencyProviderFactory
{
    private readonly IReadOnlyDictionary<string, ICurrencyProvider> _byName;

    public CurrencyProviderFactory(IEnumerable<ICurrencyProvider> providers)
    {
        _byName = providers.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
    }

    public ICurrencyProvider GetProvider(string name)
    {
        if (_byName.TryGetValue(name, out var prov))
            return prov;

        // default to Frankfurter if not found
        if (_byName.TryGetValue("Frankfurter", out var frank))
            return frank;

        throw new NotSupportedException($"Currency provider '{name}' is not registered.");
    }
}
