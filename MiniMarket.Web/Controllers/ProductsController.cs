using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMarket.Data;
using MiniMarket.Data.Models;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;
using System.Security.Claims;

namespace MiniMarket.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IOrderService _orderService;
    private readonly AppDbContext _context;


public ProductsController(
    IProductService productService,
    ICategoryService categoryService,
    IOrderService orderService,
    AppDbContext context)
    {
        _productService = productService;
        _categoryService = categoryService;
        _orderService = orderService;
        _context = context;
    }

    
    private async Task SetCategoriesAsync()
    {
        ViewBag.Categories = await _categoryService.GetAllAsync();
    }

    private async Task<(bool canRate, bool alreadyRated)> GetRatingStatus(int productId, string? userId)
    {
        if (userId == null)
            return (false, false);

        var alreadyRated = await _context.Ratings
            .AnyAsync(r => r.ProductId == productId && r.UserId == userId);

        var hasBought = await _orderService.HasUserBoughtProduct(userId, productId);

        return (hasBought && !alreadyRated, alreadyRated);
    }


    public async Task<IActionResult> Index(int page = 1, int? categoryId = null, string search = "")
    {
        int pageSize = 10;

        await SetCategoriesAsync();

        ViewBag.SelectedCategory = categoryId;
        ViewBag.Search = search;

        // 👉 взимаме продуктите (вече филтрирани в service)
        var products = await _productService.GetAllAsync(page, pageSize, categoryId, search);

        // 👉 общ брой продукти (за pagination)
        var totalProducts = await _productService.GetCountAsync(categoryId, search);

        // 👉 pagination info
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

        return View(products);
    }


    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await SetCategoriesAsync();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductDto model)
    {
        if (!ModelState.IsValid)
        {
            await SetCategoriesAsync();
            return View(model);
        }

        
        if (model.ImageFile != null)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await model.ImageFile.CopyToAsync(stream);

            model.ImageUrl = "/images/" + fileName;
        }

        await _productService.CreateAsync(model);

        return RedirectToAction(nameof(Index));
    }

    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description ?? "",
            Price = product.Price,
            CategoryName = product.Category.Name,
            ImageUrl = product.ImageUrl,
            AverageRating = _context.Ratings
                .Where(r => r.ProductId == product.Id)
                .Select(r => (double?)r.Value)
                .Average() ?? 0
        };

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var (canRate, alreadyRated) = await GetRatingStatus(id, userId);

        ViewBag.CanRate = canRate;
        ViewBag.AlreadyRated = alreadyRated;

        return View(model);
    }

    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        var model = new CreateProductDto
        {
            Name = product.Name,
            Description = product.Description ?? "",
            Price = product.Price,
            CategoryId = product.CategoryId,
            ImageUrl = product.ImageUrl
        };

        await SetCategoriesAsync();

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, CreateProductDto model)
    {
        if (!ModelState.IsValid)
        {
            await SetCategoriesAsync();
            return View(model);
        }

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        
        if (model.ImageFile != null)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await model.ImageFile.CopyToAsync(stream);

            product.ImageUrl = "/images/" + fileName;
        }

        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.CategoryId = model.CategoryId!.Value;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Rate(int productId, int value)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var alreadyRated = await _context.Ratings
            .AnyAsync(r => r.ProductId == productId && r.UserId == userId);

        if (alreadyRated)
            return RedirectToAction("Details", new { id = productId });

        var rating = new Rating
        {
            ProductId = productId,
            UserId = userId!,
            Value = value
        };

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = productId });
    }


}
