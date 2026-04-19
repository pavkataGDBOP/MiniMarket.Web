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

    public async Task<IEnumerable<ProductViewModel>> GetAllAsync(int page, int pageSize, int? categoryId, string search)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        // 🔍 search filter
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                (p.Description != null && p.Description.ToLower().Contains(search)));
        }



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
                ImageUrl = p.ImageUrl,
                AverageRating = _context.Ratings
    .Where(r => r.ProductId == p.Id)
    .Average(r => (double?)r.Value) ?? 0,
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

    public async Task<bool> HasUserBoughtProduct(string userId, int productId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId && o.IsCompleted) // optional
            .AnyAsync(o => o.OrderItems.Any(oi => oi.ProductId == productId));
    }
    //за пагинацията
    public async Task<int> GetCountAsync(int? categoryId, string search)
    {
        var query = _context.Products.AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                (p.Description != null && p.Description.ToLower().Contains(search)));
        }

        return await query.CountAsync();
    }
}