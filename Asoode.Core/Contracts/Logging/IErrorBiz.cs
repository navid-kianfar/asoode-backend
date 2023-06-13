using System;
using System.Threading.Tasks;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.Admin;
using Asoode.Core.ViewModels.General;

namespace Asoode.Core.Contracts.Logging;

public interface IErrorBiz
{
    string ExtractError(Exception ex, ErrorLogPayload payload = null);

    Task LogException(Exception ex, ErrorLogPayload payload = null);
    Task<OperationResult<GridResult<ErrorViewModel>>> AdminErrorsList(Guid userId, GridFilter model);
    Task<OperationResult<bool>> AdminErrorsDelete(Guid userId, Guid id);
}