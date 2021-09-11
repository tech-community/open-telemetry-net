using System;

namespace Payment.API.Models.DTOs
{
    public class OrderDetailDto
    {
        public int Quantity {get; set;}
        public string Product {get; set;}
        public decimal Price {get;set;}
        public decimal Subtotal {get;set;}
    }
}
