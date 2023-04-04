using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Helpers;

namespace Asoode.Application.Business.General;

internal class QueueClient : IQueueClient
{
    private readonly string QueueServer;
    private readonly string QueueUsername;
    private readonly string QueuePassword;
    private readonly string QueuePrefix;

    public QueueClient()
    {
        QueueServer = EnvironmentHelper.Get("APP_QUEUE_SERVER")!;
        QueueUsername = EnvironmentHelper.Get("APP_QUEUE_USER")!;
        QueuePassword = EnvironmentHelper.Get("APP_QUEUE_PASS")!;
        QueuePrefix = EnvironmentHelper.Get("APP_QUEUE_PREFIX")!;
    }
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task Enqueue(string queueName, string data)
    {
        throw new NotImplementedException();
    }

    public Task Enqueue(string queueName, object data, bool forceIdCheck = true)
    {
        throw new NotImplementedException();
    }

    public Task<T> Dequeue<T>(string queueName, int? timeout) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<string> Dequeue(string queueName, int? timeout)
    {
        throw new NotImplementedException();
    }

    public Task<object> Subscribe(string queueName, Func<string, Task<bool>> handler)
    {
        throw new NotImplementedException();
    }

    public Task Subscribe(string queueName)
    {
        throw new NotImplementedException();
    }

    public Task Unsubscribe()
    {
        throw new NotImplementedException();
    }

    public Task Close()
    {
        throw new NotImplementedException();
    }

    public Task Ack(string queueName)
    {
        throw new NotImplementedException();
    }

    public Task Declare(string queueName)
    {
        throw new NotImplementedException();
    }

    public string GetQueueName(string name, string lang = "en")
    {
        return $"{QueuePrefix}-{lang}-name".Trim().ToLower().Replace(' ', '-');
    }
}