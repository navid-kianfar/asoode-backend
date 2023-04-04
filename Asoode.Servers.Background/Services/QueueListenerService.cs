using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Primitives;
using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Servers.Background.Services;

internal class QueueListenerService<T, Y> : Microsoft.Extensions.Hosting.BackgroundService where T: class
{
    private readonly IJsonService _jsonService;
    private readonly IQueueClient _queueClient;
    private readonly ILoggerService _logger;
    private readonly string _queueName;

    protected QueueListenerService(
        string actionName, 
        IJsonService jsonService, 
        IQueueClient queueClient, 
        ILoggerService logger)
    {
        _jsonService = jsonService;
        _queueClient = queueClient;
        _logger = logger;
        _queueName = queueClient.GetQueueName(actionName);
    }

    private async Task ExecuteSubscriptionAsync(CancellationToken stoppingToken)
    {
        await _queueClient.Subscribe(_queueName, async (content) =>
        {
            try
            {
                var item = _jsonService.Deserialize<T>(content);
                if (item == null) throw new ApplicationException("OBJECT_IN_QUEUE_IS_NULL");
                
                var response = await ExecuteAction(item);
                return response.Status == OperationResultStatus.Success;
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, e);
                return false;
            }
        });
    }
    private async Task ExecuteDequeueAsync(CancellationToken stoppingToken)
    {
        await _queueClient.Subscribe(_queueName);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var item = await _queueClient.Dequeue<T>(_queueName, null);
                if (item == null) throw new ApplicationException("OBJECT_IN_QUEUE_IS_NULL");
                
                var response = await ExecuteAction(item);
                if (response.Status == OperationResultStatus.Success)
                {
                    // operation is successful so we remove the item from queue
                    await _queueClient.Ack(_queueName);
                }
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, e);
            }
        }
    }

    protected sealed override Task ExecuteAsync(CancellationToken stoppingToken) => 
        ExecuteSubscriptionAsync(stoppingToken);

    protected virtual Task<OperationResult<Y>> ExecuteAction(T item)
        => Task.FromResult(OperationResult<Y>.Success());
}