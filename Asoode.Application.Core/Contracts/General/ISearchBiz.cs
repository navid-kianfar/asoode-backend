using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.General.Search;

namespace Asoode.Application.Core.Contracts.General;

public interface ISearchBiz
{
    Task<OperationResult<SearchResultViewModel>> Query(string search, Guid userId);
}