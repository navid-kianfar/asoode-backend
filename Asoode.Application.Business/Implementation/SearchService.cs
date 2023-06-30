using Asoode.Application.Abstraction.Contracts;
using Asoode.Shared.Abstraction.Dtos.General.Search;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Business.Implementation;

internal class SearchService : ISearchService
{
    public Task<OperationResult<SearchResultDto>> Query(string search, Guid userId)
    {
        throw new NotImplementedException();
    }
}