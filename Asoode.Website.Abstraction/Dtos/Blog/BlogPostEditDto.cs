using System.ComponentModel.DataAnnotations;

namespace Asoode.Website.Abstraction.Dtos.Blog;

public record BlogPostEditDto
{
    public string Keywords { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string Title { get; set; }
    public string Summary { get; set; }
    [Required] public string Text { get; set; }
}