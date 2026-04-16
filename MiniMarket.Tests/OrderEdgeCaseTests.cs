using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class OrderEdgeCaseTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
       .UseInMemoryDatabase(Guid.NewGuid().ToString())
       .Options;


        return new AppDbContext(options);
    }

    [Fact]
    public async Task CompleteOrderAsync_ShouldNotThrow_WhenOrderDoesNotExist()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        await service.CompleteOrderAsync(999); // несъществуващ

        Assert.True(true); // просто да не crash-не
    }

    [Fact]
    public async Task HasUserBoughtProduct_ShouldReturnFalse_WhenUserHasNoOrders()
    {
        var context = GetDbContext();
        var service = new OrderService(context);

        var result = await service.HasUserBoughtProduct("user1", 1);

        Assert.False(result);
    }

}