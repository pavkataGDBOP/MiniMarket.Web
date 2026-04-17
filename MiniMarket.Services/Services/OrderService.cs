using Microsoft.EntityFrameworkCore;
using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Helpers;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;

namespace MiniMarket.Services.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;


public OrderService(AppDbContext context)
    {
        _context = context;
    }

    
    private static OrderViewModel MapOrder(Order o)
    {
        return new OrderViewModel
        {
            Id = o.Id,
            UserId = o.UserId,
            CreatedOn = o.CreatedOn,
            Email = o.User?.Email ?? "",
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
        };
    }


    public async Task CreateOrderAsync(string userId, List<CartItemDto> items, CheckoutDto model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 🔐 CARD LOGIC
            var cardHash = string.Empty;
            var last4 = string.Empty;

            if (model.PaymentMethod == "Card" && !string.IsNullOrEmpty(model.CardNumber))
            {
                cardHash = HashHelper.Hash(model.CardNumber);
                last4 = model.CardNumber[^4..];
            }

            var order = new Order
            {
                IsCompleted = false,
                UserId = userId,
                CreatedOn = DateTime.UtcNow,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                TotalPrice = items.Sum(x => x.Price * x.Quantity),
                PaymentMethod = model.PaymentMethod,

                // 👉 нови полета
                CardHash = cardHash,
                CardLast4 = last4,

                OrderItems = items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

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
            .Select(o => MapOrder(o))
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
            .Select(o => MapOrder(o))
            .ToListAsync();
    }

    
    public async Task<bool> HasUserBoughtProduct(string userId, int productId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId && o.IsCompleted)
            .AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == productId));
    }


}
