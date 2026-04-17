using MiniMarket.Data;
using MiniMarket.Services.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderPaymentTests
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
    public async Task CreateOrderAsync_ShouldSavePaymentMethod()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        var items = new List<CartItemDto>
    {
        new CartItemDto { ProductId = 1, Price = 5, Quantity = 1 }
    };

        var model = new CheckoutDto
        {
            FirstName = "Test",
            LastName = "User",
            Address = "Test",
            PaymentMethod = "Card"
        };

        await service.CreateOrderAsync("user1", items, model);

        var order = await context.Orders.FirstAsync();

        Assert.Equal("Card", order.PaymentMethod);
    }


}
