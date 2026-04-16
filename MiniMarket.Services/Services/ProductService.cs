using Microsoft.EntityFrameworkCore;
using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;
using System;

namespace MiniMarket.Services.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(CreateProductDto model)
    {
        var product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            CategoryId = model.CategoryId!.Value,
            ImageUrl = model.ImageUrl,


        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllAsync(int page, int pageSize, int? categoryId)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        // 🔥 ФИЛТЪР
        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        return await query
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync();
    }

    public async Task<ProductViewModel?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}