using System.ComponentModel.DataAnnotations;

namespace Asoode.Admin.Abstraction.Dtos;

public record MarketerEditableDto
{
    [Required] [MaxLength(100)] public string Code { get; set; }
    [Required] [MaxLength(2000)] public string Description { get; set; }
    public bool Enabled { get; set; }
    public int? Fixed { get; set; }
    public int? Percent { get; set; }
    [Required] [MaxLength(2000)] public string Title { get; set; }
}