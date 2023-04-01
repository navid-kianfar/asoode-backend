using Asoode.Application.Core.ViewModels.General;

namespace Asoode.Application.Core.Contracts.Logging;

public interface IErrorBiz
{
    string ExtractError(Exception ex, ErrorLogPayload payload = null);

    Task LogException(Exception ex, ErrorLogPayload payload = null);
}