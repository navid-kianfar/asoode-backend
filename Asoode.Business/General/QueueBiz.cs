using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Asoode.Business.General;

internal record QueueBiz : IQueueBiz
{
    private readonly List<string> _boundQueuesToExchange = new();
    private readonly Dictionary<string, ulong> _consumedList = new();
    private readonly List<string> _declaredQueues = new();
    private readonly string _exchange;
    private readonly string _host;
    private readonly IJsonBiz _jsonService;
    private readonly ILoggerService _loggerService;
    private readonly string _password;
    private readonly int _port;
    private readonly string _prefix;
    private readonly string _username;
    private IModel _channel;
    private IConnection _connection;
    private ConnectionFactory _factory;
    private bool _initialized;
    private IBasicProperties _properties;

    public QueueBiz(IJsonBiz jsonService, ILoggerService loggerService, IConfiguration configuration)
    {
        _loggerService = loggerService;
        _jsonService = jsonService;
        _exchange = string.Empty;
        _initialized = false;

        _prefix = configuration["Setting:MessageQueue:Prefix"]!;
        _host = configuration["Setting:MessageQueue:Server"]!;
        _username = configuration["Setting:MessageQueue:Username"]!;
        _password = configuration["Setting:MessageQueue:Password"]!;
        _port = int.Parse(configuration["Setting:MessageQueue:Port"]!);
    }

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
                await _loggerService.Error(e);
                _channel!.BasicReject(ea.DeliveryTag, true);
            }
        };
        _channel.BasicConsume(queueName, false, consumer);
        return consumer;
    }

    public string GetQueueName(string name, string lang = "en")
    {
        return $"{_prefix}-{lang}-{name}"
            .Trim()
            .ToLower()
            .Replace('.', '-')
            .Replace(' ', '-');
    }

    public Task Close()
    {
        if (_channel != null && _channel.IsOpen) _channel.Close();
        if (_connection != null && _connection.IsOpen) _connection.Close();
        return Task.CompletedTask;
    }

    public Task Declare(string queueName)
    {
        InitializeRabbitMq();
        if (_declaredQueues.Any(q => q == queueName))
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

    private void InitializeRabbitMq()
    {
        if (_initialized) return;
        _factory = new ConnectionFactory { HostName = _host, UserName = _username, Password = _password, Port = _port };
        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        _properties = _channel.CreateBasicProperties();
        _properties.Persistent = true;
        _channel.BasicQos(0, 1, false);
        _connection.ConnectionShutdown += (sender, e) => InitializeRabbitMq();
        if (!string.IsNullOrEmpty(_exchange))
            _channel.ExchangeDeclare(_exchange, ExchangeType.Topic);
        _initialized = true;
    }

    private void BindQueueToExchange(string queueName)
    {
        if (string.IsNullOrEmpty(_exchange) || _boundQueuesToExchange.Any(q => q == queueName))
            return;

        _channel.ExchangeDeclare(_exchange, ExchangeType.Topic);
        _channel.QueueBind(queueName, _exchange, string.Empty);
        _boundQueuesToExchange.Add(queueName);
    }
}