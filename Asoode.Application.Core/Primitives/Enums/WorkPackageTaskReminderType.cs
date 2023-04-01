namespace Asoode.Application.Core.Primitives.Enums;

public enum WorkPackageTaskReminderType : byte
{
    None = 1,
    AtTheTime = 2,
    FiveMinutesBefore = 3,
    TenMinutesBefore = 4,
    FifteenMinutesBefore = 5,
    OneHourBefore = 6,
    TwoHoursBefore = 7,
    OneDayBefore = 8,
    TwoDaysBefore = 9
}