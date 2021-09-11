using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notifications.API.Hubs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Notifications.API.Services
{  
public class RabbitMQService : IRabbitMQService
    {
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;
        private readonly IConfiguration _configuration;
        protected readonly IServiceProvider _serviceProvider;

        private readonly ILogger<RabbitMQService> _logger;
 
        public RabbitMQService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<RabbitMQService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceProvider = serviceProvider;

            _factory = new ConnectionFactory() { HostName = _configuration["RabbitMq:Host"] };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
 
        public virtual void Connect()
        {
            _channel.QueueDeclare(queue: "messages", durable: false, exclusive: false, autoDelete: false);
 
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += delegate (object model, BasicDeliverEventArgs ea) {
                var chatHub = (IHubContext<NotificationsHub>)_serviceProvider.GetService(typeof(IHubContext<NotificationsHub>));
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogInformation("Notificación: " + message);

                chatHub.Clients.All.SendAsync("notifications", message);
            };

            _channel.BasicConsume(queue: "messages", autoAck: true, consumer: consumer);
        }
 
    }
}