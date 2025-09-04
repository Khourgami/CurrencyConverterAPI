using System.Text.Json.Serialization;

namespace CurrencyConverter.Infrastructure.Providers.Frankfurter;

public class LatestResponse
{
    public string Base { get; set; } = default!;
    public string Date { get; set; } = default!;
    public Dictionary<string, decimal> Rates { get; set; } = new();
}


internal sealed class HistoricalResponse // used for range queries
{
    [JsonPropertyName("start_date")] public string StartDate { get; set; } = default!;
    [JsonPropertyName("end_date")] public string EndDate { get; set; } = default!;
    [JsonPropertyName("base")] public string Base { get; set; } = default!;
    [JsonPropertyName("rates")] public Dictionary<string, Dictionary<string, decimal>> RatesByDate { get; set; } = new();
}
