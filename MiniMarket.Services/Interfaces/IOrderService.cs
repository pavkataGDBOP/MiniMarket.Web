using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MiniMarket.Services.Models;

namespace MiniMarket.Services.Interfaces;

public interface IOrderService
{
    Task CreateOrderAsync(string userId, List<CartItemDto> items, CheckoutDto model);
    Task<IEnumerable<OrderViewModel>> GetAllAsync();
    Task CompleteOrderAsync(int orderId);
    Task<IEnumerable<OrderViewModel>> GetCompletedAsync();
}
