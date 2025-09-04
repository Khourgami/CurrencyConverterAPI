namespace CurrencyConverter.Domain.Primitives;

/// <summary>
/// Lightweight success/failure wrapper for domain operations (optional to use).
/// </summary>
public readonly record struct Result(bool IsSuccess, string? Error = null)
{
    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public readonly record struct Result<T>(bool IsSuccess, T? Value, string? Error)
{
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
