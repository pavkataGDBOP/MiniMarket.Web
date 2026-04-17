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

        //  Helper methods 
        private List<CartItemDto> GetCart()
        {
            return HttpContext.Session.GetObject<List<CartItemDto>>(CartKey)
                   ?? new List<CartItemDto>();
        }

        private void SaveCart(List<CartItemDto> cart)
        {
            HttpContext.Session.SetObject(CartKey, cart);
        }

        private void SetCartViewData(List<CartItemDto> cart)
        {
            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(x => x.Price * x.Quantity);
        }

       
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

       
        [Authorize]
        public async Task<IActionResult> Add(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            var cart = GetCart();

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
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                });
            }

            SaveCart(cart);

            return RedirectToAction(nameof(Index));
        }

        //  Remove product
        public IActionResult Remove(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction(nameof(Index));
        }

       
        public IActionResult Checkout()
        {
            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction(nameof(Index));

            SetCartViewData(cart);

            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDto model)
        {
            var cart = GetCart();

            if (!cart.Any())
                return RedirectToAction(nameof(Index));

            
            if (!ModelState.IsValid)
            {
                SetCartViewData(cart);
                return View(model);
            }

            
            if (model.PaymentMethod == "Card")
            {
                if (string.IsNullOrWhiteSpace(model.CVV))
                    ModelState.AddModelError("CVV", "CVV is required");

                if (string.IsNullOrWhiteSpace(model.CardNumber))
                    ModelState.AddModelError("CardNumber", "Card number is required");

                if (string.IsNullOrWhiteSpace(model.CardHolder))
                    ModelState.AddModelError("CardHolder", "Card holder is required");

                if (string.IsNullOrWhiteSpace(model.Expiry))
                    ModelState.AddModelError("Expiry", "Expiry is required");

                if (!ModelState.IsValid)
                {
                    SetCartViewData(cart);
                    return View(model);
                }
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _orderService.CreateOrderAsync(userId!, cart, model);

            HttpContext.Session.Remove(CartKey);

            TempData["SuccessMessage"] = "✅ Order placed successfully!";

            return RedirectToAction("Index", "Products");
        }

        // 🔹 Update quantity
        public IActionResult UpdateQuantity(int id, int change)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);

            if (item != null)
            {
                item.Quantity += change;

                if (item.Quantity <= 0)
                {
                    cart.Remove(item);
                }

                SaveCart(cart);
            }

            return RedirectToAction(nameof(Index));
        }
    }


}
