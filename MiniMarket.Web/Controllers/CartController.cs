using Microsoft.AspNetCore.Mvc;
using MiniMarket.Data;
using MiniMarket.Services.Interfaces;
using MiniMarket.Services.Models;
using MiniMarket.Web.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace MiniMarket.Web.Controllers
{
    
    public class CartController : Controller
    {
        private const string CartKey = "cart";

        private readonly IOrderService _orderService;
        private readonly AppDbContext _context;

        public CartController(IOrderService orderService, AppDbContext context)
        {
            _orderService = orderService;
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>(CartKey)
                       ?? new List<CartItemDto>();

            return View(cart);
        }
        [Authorize]
        public async Task<IActionResult> Add(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItemDto>>(CartKey)
                       ?? new List<CartItemDto>();

            var existing = cart.FirstOrDefault(x => x.ProductId == id);

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                cart.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetObject(CartKey, cart);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>(CartKey);

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductId == id);
                if (item != null)
                {
                    cart.Remove(item);
                }

                HttpContext.Session.SetObject(CartKey, cart);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>(CartKey);

            if (cart == null || !cart.Any())
                return RedirectToAction(nameof(Index));

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(x => x.Price * x.Quantity);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto model)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>(CartKey);

            if (cart == null || !cart.Any())
                return RedirectToAction(nameof(Index));
            if (!ModelState.IsValid)
            {
          

                ViewBag.Cart = cart;
                ViewBag.Total = cart?.Sum(x => x.Price * x.Quantity);

                return View(model);
            }

            if (model.PaymentMethod == "Card")
            {
                if (string.IsNullOrWhiteSpace(model.CardNumber) ||
                    string.IsNullOrWhiteSpace(model.CVV))
                {
                    ModelState.AddModelError("", "Card details are required");


                    return View(model);
                }
            }


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _orderService.CreateOrderAsync(userId!, cart, model);

            HttpContext.Session.Remove(CartKey);

            TempData["SuccessMessage"] = "✅ Order placed successfully!";

            return RedirectToAction("Index", "Products");
        }

        public IActionResult UpdateQuantity(int id, int change)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>(CartKey);

            if (cart != null)
            {
                var item = cart.FirstOrDefault(x => x.ProductId == id);

                if (item != null)
                {
                    item.Quantity += change;

                    if (item.Quantity <= 0)
                    {
                        cart.Remove(item);
                    }

                    HttpContext.Session.SetObject(CartKey, cart);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}