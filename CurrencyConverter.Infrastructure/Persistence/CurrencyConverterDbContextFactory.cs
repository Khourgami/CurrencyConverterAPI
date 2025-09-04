using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CurrencyConverter.Infrastructure.Persistence
{
    public class CurrencyConverterDbContextFactory : IDesignTimeDbContextFactory<CurrencyConverterDbContext>
    {
        public CurrencyConverterDbContext CreateDbContext(string[] args)
        {
            // Read configuration from appsettings.json in Api project
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../CurrencyConverter.Api"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CurrencyConverterDbContext>();
            optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

            return new CurrencyConverterDbContext(optionsBuilder.Options);
        }
    }
}
