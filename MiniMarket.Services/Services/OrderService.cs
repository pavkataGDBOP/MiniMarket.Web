using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace MiniMarket.Services.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateOrderAsync(string userId, List<CartItemDto> items, CheckoutDto model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = userId,
                CreatedOn = DateTime.UtcNow,
                OrderItems = new List<OrderItem>(),

                // ✅ новите полета
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                TotalPrice = items.Sum(x => x.Price * x.Quantity),
                PaymentMethod = model.PaymentMethod
            };

            foreach (var item in items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<IEnumerable<OrderViewModel>> GetAllAsync()
    {
        return await _context.Orders
            .Where(o => !o.IsCompleted)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedOn)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedOn = o.CreatedOn,

                // ✅ новите полета
                Email = o.User != null ? o.User.Email ?? "" : "",
                FirstName = o.FirstName,
                LastName = o.LastName,
                Address = o.Address,
                TotalPrice = o.TotalPrice,
                PaymentMethod = o.PaymentMethod,

                Items = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task CompleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);

        if (order == null)
            return;

        order.IsCompleted = true;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<OrderViewModel>> GetCompletedAsync()
    {
        return await _context.Orders
            .Where(o => o.IsCompleted)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedOn)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedOn = o.CreatedOn,
                Email = o.User!.Email!,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Address = o.Address,
                TotalPrice = o.TotalPrice,
                PaymentMethod = o.PaymentMethod,
                Items = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity
                }).ToList()
            })
            .ToListAsync();
    }


}
