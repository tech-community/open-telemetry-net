using System;

namespace Order.API.Models
{
    public class OrderDetailDto
    {
        public int Quantity {get; set;}
        public string Product {get; set;}
        public decimal Price {get;set;}
        public decimal Subtotal {get;set;}
    }
}
