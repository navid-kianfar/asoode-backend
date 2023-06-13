using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General.Search;

namespace Asoode.Core.Contracts.General;

public interface ISearchBiz
{
    Task<OperationResult<SearchResultViewModel>> Query(string search, Guid userId);
}