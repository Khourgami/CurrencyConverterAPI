using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.Primitives;

namespace CurrencyConverter.Domain.ValueObjects;

public sealed class Pagination : IEquatable<Pagination>
{
    public int Page { get; }
    public int PageSize { get; }

    public const int MinPageSize = 1;
    public const int MaxPageSize = 200;

    private Pagination(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public static Pagination Create(int page, int pageSize)
    {
        Guard.AgainstOutOfRange(page < 1, ErrorCodes.Pagination, "Page must be >= 1.");
        Guard.AgainstOutOfRange(pageSize < MinPageSize || pageSize > MaxPageSize,
            ErrorCodes.Pagination, $"PageSize must be between {MinPageSize} and {MaxPageSize}.");
        return new Pagination(page, pageSize);
    }

    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;

    public bool Equals(Pagination? other) =>
        other is not null && Page == other.Page && PageSize == other.PageSize;

    public override bool Equals(object? obj) => obj is Pagination p && Equals(p);
    public override int GetHashCode() => HashCode.Combine(Page, PageSize);
    public override string ToString() => $"page={Page}, size={PageSize}";
}
