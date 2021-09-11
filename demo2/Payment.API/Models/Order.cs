using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.API.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderID { get; set; }
        public DateTime CreationDate {get; set;}
        public string ClientName {get;set;}
        public string Address {get;set;}
        public decimal Total {get; set;}
        public string UuidTransaction {get;set;}
        public string PaymentGateway {get;set;}
        public List<OrderDetail> OrderDetail {get;set;}
    }
}