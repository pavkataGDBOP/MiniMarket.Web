using MiniMarket.Data;
using MiniMarket.Services.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderUserDataTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;


    return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldSaveUserDataCorrectly()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        var items = new List<CartItemDto>
    {
        new CartItemDto { ProductId = 1, Price = 10, Quantity = 1 }
    };

        var model = new CheckoutDto
        {
            FirstName = "Anton",
            LastName = "Mishkov",
            Address = "Sofia Center",
            PaymentMethod = "Card"
        };

        await service.CreateOrderAsync("user42", items, model);

        var order = await context.Orders.FirstAsync();

        Assert.Equal("Anton", order.FirstName);
        Assert.Equal("Mishkov", order.LastName);
        Assert.Equal("Sofia Center", order.Address);
    }


}
