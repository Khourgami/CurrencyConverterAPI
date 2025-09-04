namespace CurrencyConverter.Domain.Entities;

public class CurrencyConversionHistory
{
    public int Id { get; set; }

    public string SourceCurrency { get; set; } = default!;
    public string TargetCurrency { get; set; } = default!;
    public decimal Amount { get; set; }
    public decimal Rate { get; set; }             // exchange rate at the time
    public decimal ResultAmount { get; set; }     // converted result
    public DateTime ConvertedAt { get; set; }
}
