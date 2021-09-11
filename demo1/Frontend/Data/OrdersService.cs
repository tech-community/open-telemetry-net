using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Frontend.Models;

namespace Frontend.Data
{
    public class OrdersService : IOrdersService
    {
        private readonly HttpClient httpClient;
        public OrdersService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<OrderDto> GetOrderAsync(int OrderID)
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetStringAsync($"Payments/order/{OrderID}");

            OrderDto order = JsonSerializer.Deserialize<OrderDto>(response);

            return order;
        }
    }
}
