namespace CurrencyConverter.Application.Interfaces;

public interface ICurrencyProviderFactory
{
    ICurrencyProvider GetProvider(string name);
}
