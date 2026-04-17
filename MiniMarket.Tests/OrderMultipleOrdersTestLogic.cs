using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderMultipleOrdersTest
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w =>
                w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task HasUserBoughtProduct_ShouldWorkWithMultipleOrders()
    {
        var context = GetDbContext();

        context.Orders.AddRange(
            new Order
            {
                UserId = "user1",
                IsCompleted = true,
                FirstName = "Test",
                LastName = "User",
                Address = "Sofia",
                PaymentMethod = "Cash",
                TotalPrice = 10,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1 }
                }
            },
            new Order
            {
                UserId = "user1",
                IsCompleted = true,
                FirstName = "Test",
                LastName = "User",
                Address = "Sofia",
                PaymentMethod = "Cash",
                TotalPrice = 10,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 2 }
                }
            }
        );

        await context.SaveChangesAsync();

        var service = new OrderService(context);

        var result1 = await service.HasUserBoughtProduct("user1", 1);
        var result2 = await service.HasUserBoughtProduct("user1", 2);
        var result3 = await service.HasUserBoughtProduct("user1", 3);

        Assert.True(result1);
        Assert.True(result2);
        Assert.False(result3);
    }
}