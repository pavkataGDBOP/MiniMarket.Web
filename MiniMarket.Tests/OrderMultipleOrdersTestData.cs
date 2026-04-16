using MiniMarket.Data;
using MiniMarket.Services.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderMultipleOrdersTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;


    return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateMultipleOrders()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        var items = new List<CartItemDto>
    {
        new CartItemDto { ProductId = 1, Price = 2, Quantity = 2 }
    };

        var model = new CheckoutDto
        {
            FirstName = "Test",
            LastName = "User",
            Address = "Test",
            PaymentMethod = "Cash"
        };

        await service.CreateOrderAsync("user1", items, model);
        await service.CreateOrderAsync("user1", items, model);

        var count = await context.Orders.CountAsync();

        Assert.Equal(2, count);
    }


}
