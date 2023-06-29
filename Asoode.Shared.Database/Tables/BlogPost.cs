using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Asoode.Shared.Abstraction.Dtos.Blog;
using Asoode.Shared.Database.Tables.Base;

namespace Asoode.Shared.Database.Tables;

internal class BlogPost : BaseEntity
{
    public int Index { get; set; }
    public Guid BlogId { get; set; }
    public Guid? CategoryId { get; set; }
    [Required] [MaxLength(2)] public string Culture { get; set; }
    [Required] [MaxLength(1000)] public string Keywords { get; set; }
    [Required] [MaxLength(2000)] public string Description { get; set; }
    [Required] [MaxLength(1000)] public string Title { get; set; }
    [Required] [MaxLength(1000)] public string NormalizedTitle { get; set; }
    [Required] [MaxLength(2000)] public string Summary { get; set; }
    [MaxLength(1000)] public string EmbedCode { get; set; }
    [Required] [MaxLength(10000)] public string Text { get; set; }
    [MaxLength(500)] public string ThumbImage { get; set; }
    [MaxLength(500)] public string MediumImage { get; set; }
    [MaxLength(500)] public string LargeImage { get; set; }
    [MaxLength(20)] public string Key { get; set; }
    [DefaultValue(0)] public int Priority { get; set; }

    public PostDto ToDto()
    {
        return new PostDto
        {
            Culture = Culture,
            Description = Description,
            Keywords = Keywords,
            Index = Index,
            BlogId = BlogId,
            CategoryId = CategoryId,
            EmbedCode = EmbedCode,
            LargeImage = LargeImage,
            MediumImage = MediumImage,
            NormalizedTitle = NormalizedTitle,
            ThumbImage = ThumbImage,
            Key = Key,
            Summary = Summary,
            Text = Text,
            Title = Title,
            Id = Id,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt,
        };
    }
}