namespace CurrencyConverter.Tests.Integration.Support
{
    public sealed class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public sealed class LoginResponse
    {
        public string? Token { get; set; }
    }

    public sealed class CurrencyConversionRequest
    {
        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
        public decimal Amount { get; set; }
    }

    public sealed class CurrencyConversionResponse
    {
        public decimal OriginalAmount { get; set; }
        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
        public decimal ConvertedAmount { get; set; }
        public DateTime RateDate { get; set; }
    }
}
