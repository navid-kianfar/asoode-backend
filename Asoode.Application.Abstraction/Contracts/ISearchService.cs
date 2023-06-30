using Asoode.Shared.Abstraction.Dtos.General.Search;
using Asoode.Shared.Abstraction.Types;

namespace Asoode.Application.Abstraction.Contracts;

public interface ISearchService
{
    Task<OperationResult<SearchResultDto>> Query(string search, Guid userId);
}