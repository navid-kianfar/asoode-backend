using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.ViewModels.General;

public class TaskLogViewModel
{
    public string Description { get; set; }
    public Guid RecordId { get; set; }
    public ActivityType Type { get; set; }
    public Guid UserId { get; set; }
}