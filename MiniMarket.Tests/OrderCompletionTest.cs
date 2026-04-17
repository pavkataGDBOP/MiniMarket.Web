using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderCompletionTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CompleteOrderAsync_ShouldSetIsCompletedToTrue()
    {
        var context = GetDbContext();

        var order = new Order
        {
            UserId = "user1",
            IsCompleted = false,
            FirstName = "Test",
            LastName = "User",
            Address = "Sofia",
            PaymentMethod = "Cash",
            TotalPrice = 10
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var service = new OrderService(context);

        await service.CompleteOrderAsync(order.Id);

        var result = await context.Orders.FindAsync(order.Id);

        Assert.True(result!.IsCompleted);
    }

}