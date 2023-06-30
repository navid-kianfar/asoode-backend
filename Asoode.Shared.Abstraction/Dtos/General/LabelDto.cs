using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.General;

public record LabelDto : BaseDto
{
    [MaxLength(100)] public string Title { get; set; }
    [MaxLength(15)] public string Color { get; set; }
    public bool DarkColor { get; set; }
    public Guid PackageId { get; set; }
}