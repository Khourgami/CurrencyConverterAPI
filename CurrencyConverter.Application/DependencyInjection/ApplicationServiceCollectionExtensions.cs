using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CurrencyConverter.Application.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register all application-layer services here
            services.AddScoped<ICurrencyConversionService, CurrencyConversionService>();

            return services;
        }
    }
}
