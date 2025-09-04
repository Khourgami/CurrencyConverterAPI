using CurrencyConverter.Application.Interfaces;
using CurrencyConverter.Infrastructure.Persistence;
using CurrencyConverter.Infrastructure.Providers;
using CurrencyConverter.Infrastructure.Providers.Frankfurter;
using CurrencyConverter.Infrastructure.Repositories;
using CurrencyConverter.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.RateLimit;
using System;
using System.Net;
using System.Net.Http;

namespace CurrencyConverter.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // memory cache
            services.AddMemoryCache();

            services.AddScoped<ITokenService, TokenService>();


            // rate limit
            var rateLimitConfig = config.GetSection("Frankfurter:RateLimit");
            var limit = rateLimitConfig.GetValue<int>("Limit", 5);
            var perSeconds = rateLimitConfig.GetValue<int>("PerSeconds", 1);
            var rateLimitPolicy = Policy
                .RateLimitAsync(limit, TimeSpan.FromSeconds(perSeconds))
                .AsAsyncPolicy<HttpResponseMessage>();

            // Options for Frankfurter (bind config: "Frankfurter": { "BaseUrl": "..." })
            services.Configure<FrankfurterOptions>(config.GetSection("Frankfurter"));

            // Named HttpClient for Frankfurter provider (used by FrankfurterProvider via IHttpClientFactory.CreateClient)
            services.AddHttpClient(FrankfurterProvider.HttpClientName, client =>
            {
                var baseUrl = config["Frankfurter:BaseUrl"] ?? "https://api.frankfurter.app/";
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(rateLimitPolicy)
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            // Typed HttpClient for ICurrencyApiService (so CurrencyApiService gets injected with configured HttpClient)
            services.AddHttpClient<ICurrencyApiService, CurrencyApiService>((sp, client) =>
            {
                var baseUrl = config["Frankfurter:BaseUrl"] ?? "https://api.frankfurter.app/";
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

            // Database (EF Core)
            services.AddDbContext<CurrencyConverterDbContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            // Repositories (scoped - per request)
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();

            // Register providers (scoped) BEFORE factory so factory receives them
            services.AddScoped<ICurrencyProvider, FrankfurterProvider>();

            // Factory should be scoped (it depends on scoped providers)
            services.AddScoped<ICurrencyProviderFactory, CurrencyProviderFactory>();

            // Currency API service and other infra are already registered via AddHttpClient<ICurrencyApiService, CurrencyApiService>
            // If you for some reason still need a direct registration (not necessary), use:
            // services.AddScoped<ICurrencyApiService, CurrencyApiService>();

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
