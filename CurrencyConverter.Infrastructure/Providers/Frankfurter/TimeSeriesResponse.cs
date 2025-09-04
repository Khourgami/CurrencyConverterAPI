namespace CurrencyConverter.Infrastructure.Providers.Frankfurter
{
    public class TimeSeriesResponse
    {
        public string Base { get; set; } = default!;
        public string StartDate { get; set; } = default!;
        public string EndDate { get; set; } = default!;
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; } = new();
    }
}
