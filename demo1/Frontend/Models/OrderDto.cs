using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Frontend.Models
{
    public class OrderDto
    {
        [JsonPropertyName("orderID")]
        public int OrderID { get; set; }

        [JsonPropertyName("creationDate")]
        public DateTime CreationDate {get; set;}

        [JsonPropertyName("clientName")]
        public string ClientName {get;set;}

        [JsonPropertyName("address")]
        public string Address {get;set;}

        [JsonPropertyName("total")]
        public decimal Total {get; set;}

        [JsonPropertyName("uuidTransaction")]
        public string UuidTransaction {get;set;}

        [JsonPropertyName("paymentGateway")]
        public string PaymentGateway {get;set;}

        [JsonPropertyName("details")]
        public List<OrderDetailDto> OrderDetail {get;set;}
    }
}