namespace CurrencyConverter.Application.DTOs;

public sealed record PaginationDto(
    int Page,
    int PageSize,
    int TotalCount);
