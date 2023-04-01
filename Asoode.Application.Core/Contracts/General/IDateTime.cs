namespace Asoode.Application.Core.Contracts.General;

public interface IDateTime
{
    DateTime DateTime { get; }
    int Day { get; set; }
    int Hours { get; set; }
    int MilliSeconds { get; set; }
    int Minutes { get; set; }
    int Month { get; set; }
    int Seconds { get; set; }
    int Year { get; set; }

    IDateTime AddDays(int value);

    IDateTime AddHours(int value);

    IDateTime AddMilliSeconds(int value);

    IDateTime AddMinutes(int value);

    IDateTime AddMonths(int value);

    IDateTime AddSeconds(int value);

    IDateTime AddYears(int value);

    IDateTime Clone();

    string ToLongFormatted();

    string ToShortFormatted();
}