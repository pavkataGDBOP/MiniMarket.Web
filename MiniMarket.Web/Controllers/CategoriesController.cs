using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;

namespace MiniMarket.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _categoryService.CreateAsync(model);

        return RedirectToAction("Index", "Products");
    }
}