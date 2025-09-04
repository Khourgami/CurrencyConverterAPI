using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Domain.Entities;
using CurrencyConverter.Infrastructure.Persistence;
using CurrencyConverter.Application.Interfaces;

namespace CurrencyConverter.Infrastructure.Repositories
{
    public class CurrencyConversionHistoryRepository : ICurrencyConversionHistoryRepository
    {
        private readonly CurrencyConverterDbContext _dbContext;

        public CurrencyConversionHistoryRepository(CurrencyConverterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(CurrencyConversionHistory history)
        {
            await _dbContext.ConversionHistories.AddAsync(history);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CurrencyConversionHistory>> GetAllAsync()
        {
            return await _dbContext.ConversionHistories
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
