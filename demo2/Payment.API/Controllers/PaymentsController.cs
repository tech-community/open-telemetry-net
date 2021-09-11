using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payment.API.Models;
using Payment.API.Models.DTOs;

namespace Payment.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger<PaymentsController> _logger;
        private DemoContext _context;
        public PaymentsController(ILogger<PaymentsController> logger, DemoContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("pay")]
        public string Pay([FromBody]OrderDto order)
        {
            _logger.LogInformation("Orden en confirmación #" + order.OrderID);

            if (OrderExist(order.OrderID)){
                _logger.LogInformation($"Orden #{order.OrderID}, ya fue confirmada.");
                return $"Orden #{order.OrderID}, ya fue confirmada.";
            }

            Order item = new Order();
            item.OrderID = order.OrderID;
            item.ClientName = order.ClientName;
            item.Address = order.Address;
            item.Total = order.Total;
            item.CreationDate = order.CreationDate;
            item.UuidTransaction = Guid.NewGuid().ToString();
            item.PaymentGateway = "VISA";
            item.OrderDetail = order.Details.Select(x => new OrderDetail(){
                Price = x.Price,
                Quantity = x.Quantity,
                Product = x.Product,
                Subtotal = x.Subtotal,
                OrderID = item.OrderID
            }).ToList();

            _context.Orders.Add(item);
            _context.SaveChanges();
            
            _logger.LogInformation($"Orden #{order.OrderID} confirmada.");
            
            return $"Orden #{order.OrderID} confirmada.";
        }


        private bool OrderExist(int id){

            var item = _context.Orders.Find(id);

            return item != null ? true : false;
        }
    }
}