using CurrencyConverter.Application.DTOs;

namespace CurrencyConverter.Application.Abstractions;

public interface ICurrencyConversionService
{
    Task<ConversionResultDto> ConvertAsync(ConversionRequestDto request, CancellationToken ct = default);

}
