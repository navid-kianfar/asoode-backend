using System;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.ViewModels.Collaboration;

public class TimeOffViewModel : BaseViewModel
{
    public DateTime BeginAt { get; set; }
    public string Description { get; set; }
    public DateTime EndAt { get; set; }
    public bool IsHourly { get; set; }
    public Guid ResponderId { get; set; }
    public RequestStatus Status { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public int Index { get; set; }
}