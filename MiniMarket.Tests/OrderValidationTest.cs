using MiniMarket.Data;
using MiniMarket.Services.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderValidationTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldHandleEmptyCart()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        var items = new List<CartItemDto>(); // празна количка

        var model = new CheckoutDto
        {
            FirstName = "Ivan",
            LastName = "Petrov",
            Address = "Sofia",
            PaymentMethod = "Cash"
        };

        await service.CreateOrderAsync("user1", items, model);

        var order = await context.Orders.FirstOrDefaultAsync();

        Assert.NotNull(order);
        Assert.Equal(0, order.TotalPrice);
    }


}
