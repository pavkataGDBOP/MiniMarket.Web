using Microsoft.AspNetCore.Mvc;
using MiniMarket.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;



[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IOrderService _orderService;

    public AdminController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Orders()
    {
        var orders = await _orderService.GetAllAsync();
        return View(orders);
    }
    public async Task<IActionResult> Complete(int id)
    {
        await _orderService.CompleteOrderAsync(id);

        return RedirectToAction("Orders");
    }
    public async Task<IActionResult> Completed()
    {
        var orders = await _orderService.GetCompletedAsync();

        return View(orders);
    }
}
