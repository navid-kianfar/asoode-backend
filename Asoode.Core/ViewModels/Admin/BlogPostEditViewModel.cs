using System.ComponentModel.DataAnnotations;

namespace Asoode.Core.ViewModels.Admin;

public class BlogPostEditViewModel
{
    public string Keywords { get; set; }
    [Required] public string Description { get; set; }
    [Required] public string Title { get; set; }
    public string Summary { get; set; }
    [Required] public string Text { get; set; }
    public int Priority { get; set; }
    public string EmbedCode { get; set; }
}