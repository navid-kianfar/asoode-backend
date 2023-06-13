using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asoode.Core.Contracts.General;
using Asoode.Core.ViewModels.Logging;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Asoode.Background.Services;

public class BulkQueue
{
    private readonly IQueueBiz _queueBiz;
    private readonly string _queueName;

    public BulkQueue(IConfiguration configuration, IQueueBiz queueBiz)
    {
        _queueBiz = queueBiz;
        _queueName = queueBiz.GetQueueName("push", configuration["Setting:I18n:Default"]);
    }

    public void Notifications(List<NotificationViewModel> notifications)
    {
        foreach (var notification in notifications)
        {
            _queueBiz.Enqueue(_queueName, new
            {
                notification.Url,
                notification.Title,
                notification.Avatar,
                notification.Description,
                notification.UserId,
                CreatedAt = DateTime.UtcNow,
                Data = new
                {
                    notification.Type,
                    notification.Data,
                    Users = notification.PushUsers
                }
            });
        }
    }
}