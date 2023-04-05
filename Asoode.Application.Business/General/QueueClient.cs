using System.Text;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.Helpers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Asoode.Application.Business.General;

internal class QueueClient : IQueueClient
{
    private bool _initialized;
    private readonly string _host;
    private readonly string _prefix;
    private readonly string _username;
    private readonly string _password;
    private readonly string _exchange;

    private readonly Dictionary<string, ulong> _consumedList = new();
    private readonly List<string> _declaredQueues = new();
    private readonly List<string> _boundQueuesToExchange = new();
    private readonly IJsonService _jsonService;
    private readonly ILoggerService _logger;
    private IBasicProperties _properties;
    private readonly bool _broadcast = false;

    private ConnectionFactory _factory;
    private IConnection? _connection;
    private IModel? _channel;

    public QueueClient(IJsonService jsonService, ILoggerService logger)
    {
        _logger = logger;
        _jsonService = jsonService;
        _prefix = EnvironmentHelper.Get("APP_QUEUE_PREFIX")!;
        _host = EnvironmentHelper.Get("APP_QUEUE_SERVER")!;
        _username = EnvironmentHelper.Get("APP_QUEUE_USER")!;
        _password = EnvironmentHelper.Get("APP_QUEUE_PASS")!;
        _exchange = string.Empty;
        _initialized = false;
    }

    private void InitializeRabbitMq()
    {
        if (_initialized) return;
        _factory = new ConnectionFactory { HostName = _host, UserName = _username, Password = _password };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        _properties = _channel.CreateBasicProperties();
        _properties.Persistent = true;
        _channel.BasicQos(0, 1, false);
        _connection.ConnectionShutdown += (object? sender, ShutdownEventArgs e) => InitializeRabbitMq();
        if (!string.IsNullOrEmpty(_exchange))
            _channel.ExchangeDeclare(_exchange, GetExchangeType());
        _initialized = true;
    }

    private string GetExchangeType() => _broadcast ? ExchangeType.Fanout : ExchangeType.Topic;

    public void Dispose()
    {
        Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async Task Enqueue(string queueName, string data)
    {
        if (string.IsNullOrWhiteSpace(data)) data = "{}";
        await Declare(queueName);
        _channel.BasicPublish(
            _exchange,
            queueName,
            _properties,
            Encoding.UTF8.GetBytes(data)
        );
    }

    public Task Enqueue(string queueName, object data)
    {
        var serialized = _jsonService.Serialize(data);
        return Enqueue(queueName, serialized);
    }

    public async Task<T> Dequeue<T>(string queueName, int? timeout) where T : class
    {
        var content = await Dequeue(queueName, timeout);
        return _jsonService.Deserialize<T>(content)!;
    }

    public async Task<string> Dequeue(string queueName, int? timeout)
    {
        await Declare(queueName);
        BindQueueToExchange(queueName);
        
        var item = _channel.BasicGet(queueName, false);
        if (item == null)
        {
            if (timeout.HasValue) await Task.Delay(timeout.Value);
            return null;
        }

        _consumedList[queueName] = item.DeliveryTag;
        var content = Encoding.UTF8.GetString(item.Body.ToArray());
        return content;
    }

    public async Task<object> Subscribe(string queueName, Func<string, Task<bool>> handler)
    {
        await Declare(queueName);
        BindQueueToExchange(queueName);
        var consumer = new EventingBasicConsumer(_channel!);
        consumer.Received += async (ch, ea) =>
        {
            try
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var success = await handler.Invoke(content);
                if (success) _channel!.BasicAck(ea.DeliveryTag, false);
                else _channel!.BasicReject(ea.DeliveryTag, true);
            }
            catch (Exception e)
            {
                await _logger.Error(e.Message, e);
                _channel!.BasicReject(ea.DeliveryTag, true);
            }
        };
        _channel.BasicConsume(queueName, false, consumer);
        return consumer;
    }

    public Task Subscribe(string queueName) => Declare(queueName);

    public Task Unsubscribe() => Task.CompletedTask;

    public Task Close()
    {
        if (_channel != null && _channel.IsOpen) _channel.Close();
        if (_connection != null && _connection.IsOpen) _connection.Close();
        return Task.CompletedTask;
    }

    public Task Ack(string queueName)
    {
        if (_consumedList.ContainsKey(queueName))
        {
            _channel?.BasicAck(_consumedList[queueName], false);
            _consumedList.Remove(queueName);
        }

        return Task.CompletedTask;
    }
    
    public string GetQueueName(string name, string lang = "en")
    {
        return $"{_prefix}-{lang}-{name}"
            .Trim()
            .ToLower()
            .Replace('.', '-')
            .Replace(' ', '-');
    }

    public Task Declare(string queueName)
    {
        InitializeRabbitMq();
        if(_declaredQueues.Any(q => q == queueName))
            return Task.CompletedTask;
        _channel?.QueueDeclare(
            queueName,
            true,
            false,
            false,
            null
        );
        _declaredQueues.Add(queueName);
        return Task.CompletedTask;
    }

    private void BindQueueToExchange(string queueName)
    {
        if(string.IsNullOrEmpty(_exchange) || _boundQueuesToExchange.Any(q => q == queueName))
            return;
        
        _channel.ExchangeDeclare(_exchange, GetExchangeType());
        _channel.QueueBind(queueName, _exchange, string.Empty);
        _boundQueuesToExchange.Add(queueName);
    }
}