using Asoode.Shared.Abstraction.Dtos.Collaboration;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;

namespace Asoode.Shared.Abstraction.Dtos.General.Search;

public record SearchResultDto
{
    public MemberInfoDto[] Members { get; set; }
    public GroupDto[] Groups { get; set; }
    public ProjectDto[] Projects { get; set; }
    public SearchStorageDto Storage { get; set; }
    public SearchTaskDto[] Tasks { get; set; }
}