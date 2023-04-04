using Asoode.Application.Core;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.ViewModels.Logging;

namespace Asoode.Servers.Background.Services;

internal class ActivityBackgroundService : QueueListenerService<ActivityLogViewModel, bool>
{
    private readonly ILoggerService _logger;

    protected ActivityBackgroundService(
        IJsonService jsonService, 
        IQueueClient queueClient, 
        ILoggerService logger) : base(ApplicationConstants.ActivityQueue, jsonService, queueClient, logger)
    {
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _logger.Log("ActivityBackgroundService", "StartAsync");
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _logger.Log("ActivityBackgroundService", "StopAsync");
        await base.StartAsync(cancellationToken);
    }

    protected override Task<OperationResult<bool>> ExecuteAction(ActivityLogViewModel item)
    {
        // TODO: handle activities
        return base.ExecuteAction(item);
    }
}