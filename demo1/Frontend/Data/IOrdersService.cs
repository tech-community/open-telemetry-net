using System;
using System.Threading.Tasks;
using Frontend.Models;

namespace Frontend.Data
{
    public interface IOrdersService
    {
        Task<OrderDto> GetOrderAsync(int OrderID);
    }
}