namespace Asoode.Core.Primitives.Enums;

public enum WorkPackageTaskState : byte
{
    ToDo = 1,
    InProgress = 2,
    Done = 3,
    Paused = 4,
    Blocked = 5,
    Canceled = 6,
    Duplicate = 7,
    Incomplete = 8,
    Blocker = 9
}