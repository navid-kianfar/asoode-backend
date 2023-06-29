using Asoode.Website.Abstraction.Contracts;
using Asoode.Website.Abstraction.Dtos.General;

namespace Asoode.Website.Business.Implementation;

internal class SeoService : ISeoService
{
    public Task<SiteMapDto[]> SiteMap()
    {
        throw new NotImplementedException();
    }

    public Task<RssDto> Rss()
    {
        throw new NotImplementedException();
    }
}