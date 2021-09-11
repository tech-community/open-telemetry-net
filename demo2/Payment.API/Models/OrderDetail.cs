using System;
using System.ComponentModel.DataAnnotations;

namespace Payment.API.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        public int Quantity {get; set;}
        public string Product {get; set;}
        public decimal Price {get;set;}
        public decimal Subtotal {get;set;}
        public int OrderID { get; set; }

        public Order Order {get;set;}
    }
}
