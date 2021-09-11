using System;
using System.Collections.Generic;

namespace Payment.API.Models.DTOs
{
    public class OrderDto2
    {
        public int OrderID { get; set; }
        public DateTime CreationDate {get; set;}
        public string ClientName {get;set;}
        public string Address {get;set;}
        public decimal Total {get; set;}
        public string UuidTransaction {get;set;}
        public string PaymentGateway {get;set;}
        public List<OrderDetailDto> Details {get;set;}
    }
}