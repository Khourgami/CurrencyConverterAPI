using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CurrencyConverter.Infrastructure.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyConverterDbContext _dbContext;

        public CurrencyRepository(CurrencyConverterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveConversionAsync(CurrencyConversionHistory entity)
        {
            _dbContext.CurrencyConversionHistories.Add(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
