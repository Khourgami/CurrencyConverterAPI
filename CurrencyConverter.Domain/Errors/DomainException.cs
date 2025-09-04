using System.Runtime.Serialization;

namespace CurrencyConverter.Domain.Errors;

[Serializable]
public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string code, string message) : base(message)
        => Code = code;

    public DomainException(string code, string message, Exception? inner) : base(message, inner)
        => Code = code;

    protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        => Code = info.GetString(nameof(Code)) ?? string.Empty;

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(Code), Code);
        base.GetObjectData(info, context);
    }
}
