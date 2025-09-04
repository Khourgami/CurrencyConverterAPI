using CurrencyConverter.Application.DTOs;
using CurrencyConverter.Domain.Errors;

namespace CurrencyConverter.Application.Validators;

public static class RequestValidators
{
    public static void Validate(this ConversionRequestDto request)
    {
        if (request.Amount <= 0)
            throw new ValidationException("Amount must be greater than 0.");
    }

    public static void Validate(this HistoricalRatesRequestDto request)
    {
        if (request.Page < 1 || request.PageSize < 1)
            throw new ValidationException("Invalid pagination values.");
    }
}
