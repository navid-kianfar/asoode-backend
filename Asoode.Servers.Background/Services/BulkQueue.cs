using System.Text;
using Asoode.Application.Core.Contracts.General;
using Asoode.Application.Core.ViewModels.Logging;
using Microsoft.Extensions.Configuration;

namespace Asoode.Servers.Background.Services
{
    public class BulkQueue
    {
        private readonly IJsonBiz _jsonBiz;
        private readonly string QueueServer;
        private readonly string QueueUsername;
        private readonly string QueuePassword;
        private readonly string QueueName;
        
        public BulkQueue(IConfiguration configuration, IJsonBiz jsonBiz)
        {
            _jsonBiz = jsonBiz;
            QueueServer = configuration["Setting:MessageQueue:Server"];
            QueueUsername = configuration["Setting:MessageQueue:Username"];
            QueuePassword = configuration["Setting:MessageQueue:Password"];
            QueuePassword = configuration["Setting:MessageQueue:Password"];
            QueueName = $"{configuration["Setting:MessageQueue:Prefix"]}-{configuration["Setting:I18n:Default"]}-push";
        }
        
        public void Notifications(List<NotificationViewModel> notifications)
        {
            var factory = new ConnectionFactory()
            {
                HostName = QueueServer,
                UserName = QueueUsername,
                Password = QueuePassword
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                foreach (var notification in notifications)
                {
                    var pushData = new
                    {
                        notification.Url,
                        notification.Title,
                        notification.Avatar,
                        notification.Description,
                        notification.UserId,
                        CreatedAt = DateTime.UtcNow,
                        Data = new
                        {
                            Type = notification.Type,
                            Data = notification.Data,
                            Users = notification.PushUsers
                        }
                    };
                    var pushSerialized = _jsonBiz.Serialize(pushData);
                    if (notification.PushUsers.Any())
                    {
                        channel.QueueDeclare(
                            queue: QueueName,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null
                        );
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: QueueName,
                            basicProperties: null,
                            body: Encoding.UTF8.GetBytes(pushSerialized)
                        );
                    }
                }
            }
        }
    }
}