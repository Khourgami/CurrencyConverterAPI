using CurrencyConverter.Domain.Errors;
using CurrencyConverter.Domain.Primitives;

namespace CurrencyConverter.Domain.ValueObjects;

public sealed class DateRange : IEquatable<DateRange>
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    private DateRange(DateOnly start, DateOnly end)
    {
        Start = start;
        End = end;
    }

    public static DateRange Create(DateOnly start, DateOnly end)
    {
        if (end < start)
        {
            throw new ValidationException(
                code: "DateRange.InvalidPeriod",
                message: $"End date '{end}' cannot be before start date '{start}'."
            );
        }

        return new DateRange(start, end);
    }

    public int Days => End.DayNumber - Start.DayNumber + 1;

    public IEnumerable<DateOnly> EnumerateDays()
    {
        for (var d = Start; d <= End; d = d.AddDays(1))
            yield return d;
    }

    public bool Contains(DateOnly date) => date >= Start && date <= End;
    public bool Equals(DateRange? other) =>
        other is not null && Start == other.Start && End == other.End;

    public override bool Equals(object? obj) => obj is DateRange r && Equals(r);
    public override int GetHashCode() => HashCode.Combine(Start, End);
    public override string ToString() => $"{Start:yyyy-MM-dd}..{End:yyyy-MM-dd}";
}
