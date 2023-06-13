using System.Threading.Tasks;
using Asoode.Core.ViewModels.Logging;

namespace Asoode.Core.Contracts.Logging;

public interface IActivityBiz
{
    Task Enqueue(ActivityLogViewModel model);
}