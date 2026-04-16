using MiniMarket.Data;
using MiniMarket.Services.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderItemsTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;


    return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldSaveAllOrderItems()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        var items = new List<CartItemDto>
    {
        new CartItemDto { ProductId = 1, Price = 10, Quantity = 1 },
        new CartItemDto { ProductId = 2, Price = 20, Quantity = 2 }
    };

        var model = new CheckoutDto
        {
            FirstName = "Test",
            LastName = "User",
            Address = "Test",
            PaymentMethod = "Cash"
        };

        await service.CreateOrderAsync("user1", items, model);

        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstAsync();

        Assert.Equal(2, order.OrderItems.Count);
    }


}
