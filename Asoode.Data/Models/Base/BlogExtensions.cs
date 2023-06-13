using Asoode.Core.ViewModels.Admin;

namespace Asoode.Data.Models.Base;

public static class BlogExtensions
{
    public static BlogViewModel ToViewModel(this Blog blog)
    {
        return new BlogViewModel
        {
            Culture = blog.Culture,
            Description = blog.Description,
            Keywords = blog.Keywords,
            Title = blog.Title,
            Type = blog.Type,
            Id = blog.Id,
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt
        };
    }

    public static PostViewModel ToViewModel(this BlogPost post)
    {
        return new PostViewModel
        {
            Key = post.Key,
            Culture = post.Culture,
            Description = post.Description,
            Id = post.Id,
            Keywords = post.Keywords,
            Summary = post.Summary,
            EmbedCode = post.EmbedCode,
            Text = post.Text,
            Title = post.Title,
            CreatedAt = post.CreatedAt,
            LargeImage = post.LargeImage,
            UpdatedAt = post.UpdatedAt,
            BlogId = post.BlogId,
            CategoryId = post.CategoryId,
            MediumImage = post.MediumImage,
            NormalizedTitle = post.NormalizedTitle,
            ThumbImage = post.ThumbImage,
            Priority = post.Priority
        };
    }
}