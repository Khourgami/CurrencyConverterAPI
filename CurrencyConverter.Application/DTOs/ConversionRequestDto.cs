namespace CurrencyConverter.Application.DTOs
{
    /// <summary>
    /// DTO representing a currency conversion request in the Application layer
    /// </summary>
    public class ConversionRequestDto
    {
        /// <summary>
        /// Source currency code (e.g., "USD")
        /// </summary>
        public string From { get; set; } = default!;

        /// <summary>
        /// Target currency code (e.g., "EUR")
        /// </summary>
        public string To { get; set; } = default!;

        /// <summary>
        /// Amount to convert
        /// </summary>
        public decimal Amount { get; set; }

        public string? Provider { get; set; } = "Frankfurter";

    }
}
