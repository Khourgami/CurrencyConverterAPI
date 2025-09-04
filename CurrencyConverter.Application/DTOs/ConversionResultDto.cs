namespace CurrencyConverter.Application.DTOs;

public sealed record ConversionResultDto(
    decimal OriginalAmount,
    string From,
    string To,
    decimal ConvertedAmount,
    DateTime RateDate)
{
    public decimal ResultAmount => OriginalAmount * ConvertedAmount;
}
