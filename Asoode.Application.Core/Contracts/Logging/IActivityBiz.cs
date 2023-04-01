using Asoode.Application.Core.ViewModels.Logging;

namespace Asoode.Application.Core.Contracts.Logging;

public interface IActivityBiz
{
    Task Enqueue(ActivityLogViewModel model);
}