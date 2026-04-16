using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMarket.Data;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;
using System.Threading.Tasks;

namespace MiniMarket.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly AppDbContext _context;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        AppDbContext context)
    {
        _productService = productService;
        _categoryService = categoryService;
        _context = context;
    }
    public async Task<IActionResult> Index(int page = 1, int? categoryId = null, string search = "")
    {
        ViewBag.Categories = await _categoryService.GetAllAsync();
        ViewBag.SelectedCategory = categoryId;
        ViewBag.Search = search;

        var products = await _productService.GetAllAsync(page, 10, categoryId);

        // 🔍 SEARCH
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();

            products = products.Where(p =>
                p.Name.ToLower().Contains(search) ||
                (p.Description != null && p.Description.ToLower().Contains(search))
            );
        }

        return View(products);
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _categoryService.GetAllAsync();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(model);
        }

        // 📸 upload image
        if (model.ImageFile != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            // 👉 записваме пътя в DTO
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
            ImageUrl = product.ImageUrl
        };

        return View(model);
    }
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

        ViewBag.Categories = await _categoryService.GetAllAsync();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CreateProductDto model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(model);
        }

        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound();

        // 📸 ако има нова снимка
        if (model.ImageFile != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            var fileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ImageFile.CopyToAsync(stream);
            }

            product.ImageUrl = "/images/" + fileName;
        }

        // 🧠 update данни
        product.Name = model.Name;
        product.Description = model.Description;
        product.Price = model.Price;
        product.CategoryId = model.CategoryId!.Value;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}