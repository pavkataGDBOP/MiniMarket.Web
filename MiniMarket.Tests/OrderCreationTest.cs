
namespace MiniMarket.Tests
{
    using MiniMarket.Data;
    using MiniMarket.Data.Models;
    using MiniMarket.Services.Models;
    using MiniMarket.Services.Services;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class OrderServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
            return new AppDbContext(options);
        }

    }
}