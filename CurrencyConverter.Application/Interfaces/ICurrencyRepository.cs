using CurrencyConverter.Domain.Entities;
using System.Threading.Tasks;

namespace CurrencyConverter.Application.Interfaces
{
    public interface ICurrencyRepository
    {
        Task SaveConversionAsync(CurrencyConversionHistory entity);
    }
}
