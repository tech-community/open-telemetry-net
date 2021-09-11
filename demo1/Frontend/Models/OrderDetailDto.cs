using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Frontend.Models
{
    public class OrderDetailDto
    {
        [JsonPropertyName("quantity")]
        public int Quantity {get; set;}

        [JsonPropertyName("product")]
        public string Product {get; set;}

        [JsonPropertyName("price")]
        public decimal Price {get;set;}

        [JsonPropertyName("subtotal")]
        public decimal Subtotal {get;set;}
    }
}
