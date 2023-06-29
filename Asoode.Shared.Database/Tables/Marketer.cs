using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Marketer;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class Marketer : BaseEntity
{
    [Required] [MaxLength(100)] public string Code { get; set; }
    [Required] [MaxLength(2000)] public string Description { get; set; }
    public bool Enabled { get; set; }
    public int? Fixed { get; set; }
    public int? Percent { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }

    public MarketerDto ToDto()
    {
        return new MarketerDto
        {
            Code = Code,
            Description = Description,
            Enabled = Enabled,
            Fixed = Fixed,
            Percent = Percent,
            Title = Title,
            Id = Id,
            // Index = Index,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }
}