using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos;

public record LabelDto
{
    [MaxLength(100)] public string Title { get; set; }
    [MaxLength(15)] public string Color { get; set; }
}