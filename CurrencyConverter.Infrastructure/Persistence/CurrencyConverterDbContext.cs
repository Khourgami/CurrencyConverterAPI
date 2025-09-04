using Microsoft.EntityFrameworkCore;
using CurrencyConverter.Domain.Entities;

namespace CurrencyConverter.Infrastructure.Persistence
{
    public class CurrencyConverterDbContext : DbContext
    {
        public CurrencyConverterDbContext(DbContextOptions<CurrencyConverterDbContext> options)
            : base(options) { }

        public DbSet<CurrencyConversionHistory> ConversionHistories { get; set; }
        public DbSet<CurrencyConversionHistory> CurrencyConversionHistories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CurrencyConverterDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
