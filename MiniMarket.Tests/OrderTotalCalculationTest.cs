using MiniMarket.Data;
using MiniMarket.Services.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderTotalCalculationTests
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
    public async Task CreateOrderAsync_ShouldCalculateTotalCorrectly_WithMultipleItems()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        var items = new List<CartItemDto>
    {
        new CartItemDto { ProductId = 1, Price = 3, Quantity = 3 }, // 9
        new CartItemDto { ProductId = 2, Price = 2, Quantity = 5 }  // 10
    };

        var model = new CheckoutDto
        {
            FirstName = "Test",
            LastName = "User",
            Address = "Test",
            PaymentMethod = "Cash"
        };

        await service.CreateOrderAsync("user1", items, model);

        var order = await context.Orders.FirstAsync();

        Assert.Equal(19, order.TotalPrice);
    }


}
