using Asoode.Shared.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Enums;
using Asoode.Shared.Abstraction.Helpers;
using Asoode.Website.Abstraction.Contracts;
using Asoode.Website.Abstraction.Dtos.General;

namespace Asoode.Website.Business.Implementation;

internal class SeoService : ISeoService
{
    private readonly IBlogService _blogService;

    public SeoService(IBlogService blogService)
    {
        _blogService = blogService;
    }

    public async Task<SiteMapDto[]> SiteMap()
    {
        var lastModified = new DateTime(2020, 11, 9, 10, 20, 30);
        var culture = EnvironmentHelper.Culture;
        var endpoint = EnvironmentHelper.Get("APP_ENDPOINT")!;
        var domain = EnvironmentHelper.Get("APP_DOMAIN")!;
        var domainWithLang = $"{domain}/{culture}";
        var result = new List<SiteMapDto>
        {
            new SiteMapDto { Location = $"{domain}", Priority = "1.0", LastModified = lastModified },
            new SiteMapDto { Location = $"{domainWithLang}", Priority = "0.9", LastModified = lastModified },
            new SiteMapDto { Location = $"{domainWithLang}/why", Priority = "0.8", LastModified = lastModified },
            // new SiteMapDto{ Location = $"{domainWithLang}/plans", Priority = "0.8", LastModified = lastModified},
            new SiteMapDto { Location = $"{domainWithLang}/faq", Priority = "0.8", LastModified = lastModified },
            new SiteMapDto
                { Location = $"{domainWithLang}/contact", Priority = "0.8", LastModified = lastModified },
            new SiteMapDto { Location = $"{domainWithLang}/about", Priority = "0.8", LastModified = lastModified },
            new SiteMapDto { Location = $"{domainWithLang}/blog", Priority = "0.8", LastModified = lastModified },
            new SiteMapDto
            {
                Location = $"{endpoint}/login?lang={culture}", Priority = "0.8",
                LastModified = lastModified
            },
            new SiteMapDto
            {
                Location = $"{endpoint}/register?lang={culture}", Priority = "0.8",
                LastModified = lastModified
            },
        };

        var posts = await _blogService.AllPosts(culture);
        foreach (var post in posts.Data!)
        {
            result.Add(new SiteMapDto
            {
                Priority = "0.7",
                Location = post.Permalink(domain),
                LastModified = post.CreatedAt,
                Title = post.Title
            });
        }

        return result.ToArray();
    }

    public async Task<RssDto> Rss()
    {
        var culture = EnvironmentHelper.Culture;
        var domain = EnvironmentHelper.Get("APP_DOMAIN")!;
        var domainWithLang = $"{domain}/{culture}";
        var channels = await _blogService.AllBlogs(culture);
        var posts = await _blogService.AllPosts(culture);

        var blog = channels.Data!.Single(b => b.Type == BlogType.Post);
        return new RssDto
        {
            Location = $"{domain}/rss.xml",
            Description = blog.Description,
            Link = blog.Permalink(domainWithLang),
            Title = blog.Title,
            Items = posts.Data!
                .OrderByDescending(i => i.CreatedAt)
                .Select(p => new RssItemDto
                {
                    Id = p.Id,
                    Description = p.Description,
                    Link = p.Permalink(domain),
                    Title = p.Title,
                    CreatedAt = p.CreatedAt
                })
                .ToArray()
        };
    }
}