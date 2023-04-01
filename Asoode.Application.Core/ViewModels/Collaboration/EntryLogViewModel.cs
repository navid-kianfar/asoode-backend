using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.ViewModels.Collaboration;

public class EntryLogViewModel : BaseViewModel
{
    public DateTime BeginAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string Duration { get; set; }
    public int Index { get; set; }

    public string FullName { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
}