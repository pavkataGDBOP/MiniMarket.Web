using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderPurchaseTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;


    return new AppDbContext(options);
    }

    [Fact]
    public async Task HasUserBoughtProduct_ShouldReturnTrue_WhenCompletedOrderExists()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            UserId = "user1",
            IsCompleted = true,
            OrderItems = new List<OrderItem>
        {
            new OrderItem { ProductId = 1 }
        }
        });

        await context.SaveChangesAsync();

        var service = new OrderService(context);

        var result = await service.HasUserBoughtProduct("user1", 1);

        Assert.True(result);
    }

    [Fact]
    public async Task HasUserBoughtProduct_ShouldReturnFalse_WhenNotCompleted()
    {
        var context = GetDbContext();

        context.Orders.Add(new Order
        {
            UserId = "user1",
            IsCompleted = false,
            OrderItems = new List<OrderItem>
        {
            new OrderItem { ProductId = 1 }
        }
        });

        await context.SaveChangesAsync();

        var service = new OrderService(context);

        var result = await service.HasUserBoughtProduct("user1", 1);

        Assert.False(result);
    }


}
