namespace Asoode.Shared.Abstraction.Contracts;

public interface IQueueService : IDisposable
{
    Task Enqueue(string queueName, string data);
    Task Enqueue(string queueName, object data);
    Task<object> Subscribe(string queueName, Func<string, Task<bool>> handler);
    string GetQueueName(string name, string lang = "en");
}