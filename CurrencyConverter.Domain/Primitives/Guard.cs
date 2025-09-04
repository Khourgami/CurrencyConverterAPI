using CurrencyConverter.Domain.Errors;

namespace CurrencyConverter.Domain.Primitives;

public static class Guard
{
    public static void AgainstNull<T>(T? value, string name)
    {
        if (value is null)
            throw new ValidationException($"{name} must not be null.");
    }

    public static void AgainstNullOrWhiteSpace(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"{name} must not be empty.");
    }

    // Keep existing 2-arg version for convenience
    public static void AgainstOutOfRange(bool condition, string message)
    {
        if (condition) throw new ValidationException(message);
    }

    // NEW overload: condition + code + message
    public static void AgainstOutOfRange(bool condition, string code, string message)
    {
        if (condition) throw new ValidationException(code, message);
    }
}
