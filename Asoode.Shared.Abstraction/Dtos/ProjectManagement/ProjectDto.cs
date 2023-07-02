using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Abstraction.Dtos.Storage;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.ProjectManagement;

public record ProjectDto : BaseDto
{
    public int AttachmentSize { get; set; }

    public Guid UserId { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Complex { get; set; }
    public ProjectTemplate Template { get; set; }
    public bool Premium { get; set; }

    public ProjectMemberDto[] Members { get; set; }
    public ProjectSeasonDto[] Seasons { get; set; }
    public SubProjectDto[] SubProjects { get; set; }
    public WorkPackageDto[] WorkPackages { get; set; }
    public PendingInvitationDto[] Pending { get; set; }

    public ExplorerFolderDto ToExplorerDto()
    {
        return new ExplorerFolderDto
        {
            Name = Title,
            Parent = "/",
            Path = $"/project/{(Complex ? "c" : "s")}/{Id}",
            CreatedAt = CreatedAt
        };
    }
}