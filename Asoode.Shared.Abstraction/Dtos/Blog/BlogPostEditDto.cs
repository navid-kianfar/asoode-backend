using System.ComponentModel.DataAnnotations;

namespace Asoode.Shared.Abstraction.Dtos.Blog;

public record BlogPostEditDto
{
    public string ThumbImage { get; set; } = string.Empty;
    public string MediumImage { get; set; } = string.Empty;
    public string LargeImage { get; set; } = string.Empty;
    
    public string Keywords { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    [Required] public string Text { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string EmbedCode { get; set; } = string.Empty;
}