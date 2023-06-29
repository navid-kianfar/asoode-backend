using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Shared.Abstraction.Dtos.Blog;

public record BlogEditDto
{
    [Required] public BlogType Type { get; set; }
    public string Keywords { get; set; }
    [Required] [MaxLength(2)] public string Culture { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string Title { get; set; }
}