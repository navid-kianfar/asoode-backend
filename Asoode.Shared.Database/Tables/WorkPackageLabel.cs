using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.General;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class WorkPackageLabel : BaseEntity
{
    [Required] public Guid PackageId { get; set; }
    [MaxLength(250)] public string Title { get; set; }

    [MaxLength(100)] public string Color { get; set; }
    public bool DarkColor { get; set; }

    public LabelDto ToDto()
    {
        return new LabelDto
        {
            Title = Title,
            DarkColor = DarkColor,
            Color = Color,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
            PackageId = PackageId
        };
    }
}