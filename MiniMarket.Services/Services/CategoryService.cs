using Microsoft.EntityFrameworkCore;
using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;

namespace MiniMarket.Services.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();

    }
    public async Task CreateAsync(CreateCategoryDto model)
    {
        var category = new Category
        {
            Name = model.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

}