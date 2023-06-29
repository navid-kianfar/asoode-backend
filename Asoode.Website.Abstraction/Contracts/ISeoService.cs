using Asoode.Website.Abstraction.Dtos.General;

namespace Asoode.Website.Abstraction.Contracts;

public interface ISeoService
{
    Task<SiteMapDto[]> SiteMap();
    Task<RssDto> Rss();
}