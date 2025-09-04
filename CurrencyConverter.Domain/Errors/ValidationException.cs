public sealed class ValidationException : Exception
{
    public string? Code { get; }

    public ValidationException(string message) : base(message) { }

    public ValidationException(string code, string message) : base(message)
    {
        Code = code;
    }
}
