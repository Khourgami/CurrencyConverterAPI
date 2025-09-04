using System.Net;
using System.Net.Http.Headers;
using System.Text;
using CurrencyConverter.Tests.Integration.Infrastructure;
using CurrencyConverter.Tests.Integration.Support;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace CurrencyConverter.Tests.Integration.Api
{
    public class AuthAndCurrencyTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthAndCurrencyTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private static StringContent JsonBody(object payload) =>
            new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        private async Task<string> LoginAndGetTokenAsync(HttpClient client, string username, string password)
        {
            var req = new LoginRequest { Username = username, Password = password };
            var resp = await client.PostAsync("/api/Auth/login", JsonBody(req));
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await resp.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<LoginResponse>(json);
            obj.Should().NotBeNull();
            obj!.Token.Should().NotBeNullOrWhiteSpace();

            return obj.Token!;
        }

        [Fact]
        public async Task Convert_Returns401_When_Missing_Bearer_Token()
        {
            // Arrange
            var client = _factory.CreateClient(); // no auth header

            var payload = new CurrencyConversionRequest
            {
                From = "USD",
                To = "EUR",
                Amount = 100
            };

            // Act
            var resp = await client.PostAsync("/api/Currency/convert", JsonBody(payload));

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_And_Convert_As_User_Succeeds()
        {
            // Arrange
            var client = _factory.CreateClient();

            // 1) Login
            var token = await LoginAndGetTokenAsync(client, "user", "password");

            // 2) Call protected endpoint
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new CurrencyConversionRequest
            {
                From = "USD",
                To = "EUR",
                Amount = 250
            };

            // Act
            var resp = await client.PostAsync("/api/Currency/convert", JsonBody(payload));

            // Assert
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await resp.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<CurrencyConversionResponse>(json);
            obj.Should().NotBeNull();
            obj!.From.Should().Be("USD");
            obj.To.Should().Be("EUR");
            obj.OriginalAmount.Should().Be(250m);
            obj.ConvertedAmount.Should().Be(250m * 1.2m); // from FakeCurrencyConversionService
        }

        [Fact]
        public async Task Login_As_Admin_Then_Convert_Succeeds()
        {
            var client = _factory.CreateClient();

            var token = await LoginAndGetTokenAsync(client, "admin", "password");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new CurrencyConversionRequest
            {
                From = "EUR",
                To = "USD",
                Amount = 50
            };

            var resp = await client.PostAsync("/api/Currency/convert", JsonBody(payload));
            resp.StatusCode.Should().Be(HttpStatusCode.OK);

            var json = await resp.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<CurrencyConversionResponse>(json);
            obj.Should().NotBeNull();
            obj!.ConvertedAmount.Should().Be(60m);
        }
    }
}
