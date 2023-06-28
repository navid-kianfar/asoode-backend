using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Admin.Abstraction.Dtos;

public record BlogEditDto
{
    [Required] public BlogType Type { get; set; }
    public string Keywords { get; set; }
    [Required] [MaxLength(2)] public string Culture { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string Title { get; set; }
}