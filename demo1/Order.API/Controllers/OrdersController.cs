using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Order.API.Models;
using RabbitMQ.Client;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private static readonly ActivitySource Activity = new(nameof(OrdersController));
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private readonly ILogger<OrdersController> _logger;
        private readonly IConfiguration _configuration;
        private string queueHistory = "history";
        public OrdersController(ILogger<OrdersController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("confirm/{id}")]
        public async Task<string> ConfirmOrder(int id)
        {
            OrderDto item = new OrderDto(){
                OrderID = id,
                ClientName = GetRandomName(),
                CreationDate = DateTime.Now,
                Address = "City"
            };

            item.Details = GetRandomDetails();
            item.Total = item.Details.Sum(x => x.Subtotal);

            _logger.LogInformation($"Orden #{item.OrderID}. Enviando mensaje a Histórico a RabbitMQ");

            SendQueueHistory(item);

            // Payload

            var stringPayload = JsonSerializer.Serialize(item);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");


            _logger.LogInformation($"Llamando a Payment.API: {_configuration["PaymentAPI_URL"]}");
            var response  = await _httpClientFactory
                .CreateClient()
                .PostAsync($"{_configuration["PaymentAPI_URL"]}/payments/pay", httpContent);

            if (response.Content != null) {
                return await response.Content.ReadAsStringAsync();
            }else{
                return "No hubo respuesta :(";
            }
        }

        private string GetRandomName(){
            var rng = new Random();
            return $"{Names[rng.Next(Names.Length)]} {Lastnames[rng.Next(Lastnames.Length)]}";
        }

        private List<OrderDetailDto> GetRandomDetails(){
            var rng = new Random();
            var cntItems = rng.Next(1,8);
            List<OrderDetailDto> lstItems = new List<OrderDetailDto>();

            for (int x = 0; x <= cntItems;x++){
                OrderDetailDto detail = new OrderDetailDto(){
                    Quantity = rng.Next(1,5),
                    Product = $"{Products[rng.Next(Products.Length)]}",
                    Price = rng.Next(100, 800),
                };
                detail.Subtotal = detail.Quantity * detail.Price;

                lstItems.Add(detail);
            }

            return lstItems;
        }


        private static readonly string[] Names = new[]
        {
            "Alex", "Daniela", "Laura", "Luis", "Julio", "John", "Mauricio", "Alexander", "Valeria", "Morgan"
        };

        private static readonly string[] Lastnames = new[]
        {
            "Gutierrez", "Aguirre", "Guzmán", "Cándara", "Álvarez", "Marshall", "Hernández", "Fernández", "López", "Sagastume"
        };

        private static readonly string[] Products = new[]
        {
            "Huawei nova 3", "Alpha Taurus 10 Pro", "Samsung Galaxy S11", "Iphone 12 Pro Max", "Huawei P40 Pro", "Samsung A10", "Xiaomi Mi 8", "Xiaomi Redmi Note 9"
        };

        private void SendQueueHistory(OrderDto item){
            try
            {
                using (var activity = Activity.StartActivity("Publicando a RabbitMQ", ActivityKind.Producer))
                {
                    var factory = new ConnectionFactory { HostName = _configuration["RabbitMq:Host"] };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        var props = channel.CreateBasicProperties();

                        //AddActivityToHeader(activity, props);

                        channel.QueueDeclare(queue: queueHistory,
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item));

                        _logger.LogInformation($"Publicando mensaje a cola: {queueHistory}");
                        channel.BasicPublish(exchange: "",
                            routingKey: queueHistory,
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

        private void AddActivityToHeader(Activity activity, IBasicProperties props)
        {
            Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), props, InjectContextIntoHeader);
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.rabbitmq.queue", queueHistory);
        }

        private void InjectContextIntoHeader(IBasicProperties props, string key, string value)
        {
            try
            {
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[key] = value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to inject trace context.");
            }
        }
    }
}
