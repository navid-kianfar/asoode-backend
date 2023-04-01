using Asoode.Application.Core.Primitives;

namespace Asoode.Application.Core.Contracts.General;

public interface IPdfBiz
{
    Task<OperationResult<bool>> FromHtml(string html, string destination);
}