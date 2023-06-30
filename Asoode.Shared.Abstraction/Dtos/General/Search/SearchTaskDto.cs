using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.General.Search;

public record SearchTaskDto : BaseDto
{

    public WorkPackageTaskState State { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string List { get; set; }
    public string WorkPackage { get; set; }
    public string Project { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid WorkPackageId { get; set; }
    public Guid ProjectId { get; set; }
    public TaskLabelDto[] Labels { get; set; }
    public MemberInfoDto[] Members { get; set; }
}