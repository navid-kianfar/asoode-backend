using Asoode.Shared.Abstraction.Dtos;
using Asoode.Shared.Abstraction.Enums;

namespace Asoode.Website.Abstraction.Dtos.Blog;

public record BlogDto : BaseDto
{
    public int Index { get; set; }

    public BlogType Type { get; set; }
    public string Culture { get; set; }
    public string Keywords { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }

    public string Permalink(string domain)
    {
        switch (Type)
        {
            case BlogType.Page: return $"{domain}/pages/";
            case BlogType.Faq: return $"{domain}/faq";
            default: return $"{domain}/blog";
        }
    }
}