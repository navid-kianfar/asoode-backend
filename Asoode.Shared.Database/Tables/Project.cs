using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.ProjectManagement;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Project : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
    [MaxLength(2000)] public string Description { get; set; }
    public bool Complex { get; set; }
    public bool Premium { get; set; }
    public ProjectTemplate Template { get; set; }
    public Guid PlanInfoId { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public ProjectDto ToDto()
    {
        return new ProjectDto
        {
            ArchivedAt = ArchivedAt,
            Description = Description,
            Complex = Complex,
            Template = Template,
            Premium = Premium,
            Title = Title,
            UserId = UserId,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            // Members = Members,
            // Pending = Pending,
            // Seasons = Seasons,
            // AttachmentSize = AttachmentSize,
            // SubProjects = SubProjects,
            // WorkPackages = WorkPackages,
        };
    }
}