
namespace MiniMarket.Services.Interfaces;


using MiniMarket.Services.Models;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task CreateAsync(CreateCategoryDto model);
}
