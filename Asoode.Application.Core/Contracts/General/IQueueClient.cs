namespace Asoode.Application.Core.Contracts.General;

public interface IQueueClient : IDisposable
{
    Task Enqueue(string queueName, string data);
    Task Enqueue(string queueName, object data, bool forceIdCheck = true);
    Task<T> Dequeue<T>(string queueName, int? timeout = 50) where T : class;
    Task<string> Dequeue(string queueName, int? timeout = 50);
    Task<object> Subscribe(string queueName, Func<string, Task<bool>> handler);
    Task Subscribe(string queueName);
    Task Unsubscribe();
    Task Close();
    Task Ack(string queueName);
    Task Declare(string queueName);

    string GetQueueName(string name, string lang = "en");
}