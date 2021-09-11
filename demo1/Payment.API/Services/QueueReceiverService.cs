using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Payment.API.Models.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
namespace Payment.API.Services
{
    public class QueueReceiverService : BackgroundService
    {
        private IModel _channel;
        private IConnection _connection;
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _queueName2;
        private readonly ILogger<QueueReceiverService> _logger;

        public QueueReceiverService(IOptions<RabbitMqConfiguration> rabbitMqOptions, ILogger<QueueReceiverService> logger)
        {
            _hostname = rabbitMqOptions.Value.Hostname;
            _queueName = rabbitMqOptions.Value.QueueName;
            _queueName2 = rabbitMqOptions.Value.QueueName2;
            _logger = logger;
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname
            };

            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var order = JsonSerializer.Deserialize<OrderDto>(content);
                _logger.LogInformation($"Orden #{order.OrderID}. Mensaje histórico recibido. ");

                string msj = $"Orden #{order.OrderID} en proceso de confirmación.";
                NotificationDto notification = new NotificationDto();
                notification.Type = "Initial";
                notification.Message = msj;
                notification.OrderID = order.OrderID;
                notification.Date = DateTime.Now;

                SendMessageQueue(notification);
                _channel.BasicAck(ea.DeliveryTag, false);
            };
            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerCancelled;

            _channel.BasicConsume(_queueName, false, consumer);

            return Task.CompletedTask;
        }

        private void SendMessageQueue(NotificationDto item){
            try
            {
                 var factory = new ConnectionFactory { HostName = _hostname };

                using (var connection = factory.CreateConnection()){
                    using (var channel = connection.CreateModel())
                    {
                        var props = channel.CreateBasicProperties();

                        channel.QueueDeclare(queue: _queueName2,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
                        
                        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item));

                        _logger.LogInformation($"Publicando mensaje a cola: {_queueName2}");
                        channel.BasicPublish(exchange: "",
                            routingKey: _queueName2,
                            basicProperties: props,
                            body: body);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error al publicar un mensaje en cola.", e);
                throw;
            }
        }

        private void OnConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}