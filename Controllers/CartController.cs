using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopEaseDbContext _context;

        public CartController(ShopEaseDbContext context)
        {
            _context = context;
        }


        // =========================================
        // VIEW CART
        // =========================================

        public async Task<IActionResult> Index()
        {
            // For now, use CustomerID = 1
            // until we build customer login.

            int customerId = 1;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(
                    c => c.CustomerID == customerId
                      && c.Status == "Active"
                );

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerID = customerId,
                    Status = "Active",
                    CartDate = DateTime.Now
                };

                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();
            }

            return View(cart);
        }


        // =========================================
        // ADD TO CART
        // =========================================

        [HttpPost]
        public async Task<IActionResult> AddToCart(
            int productId,
            int quantity = 1)
        {
            int customerId = 1;

            // Find product
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == productId);

            if (product == null)
            {
                return NotFound();
            }


            // Check stock
            if (product.StockQuantity < quantity)
            {
                TempData["Error"] =
                    "Not enough stock available.";

                return RedirectToAction(
                    "Index",
                    "Products"
                );
            }


            // Find active cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(
                    c => c.CustomerID == customerId
                      && c.Status == "Active"
                );


            // Create cart if it doesn't exist
            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerID = customerId,
                    CartDate = DateTime.Now,
                    Status = "Active"
                };

                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();
            }


            // Check if product is already in cart
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(
                    ci => ci.CartID == cart.CartID
                      && ci.ProductID == productId
                );


            if (cartItem != null)
            {
                // Existing item → increase quantity

                int newQuantity =
                    cartItem.Quantity + quantity;


                if (newQuantity > product.StockQuantity)
                {
                    TempData["Error"] =
                        "You cannot add more than the available stock.";

                    return RedirectToAction(
                        "Index",
                        "Products"
                    );
                }


                cartItem.Quantity = newQuantity;
            }
            else
            {
                // New item

                cartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity
                };

                _context.CartItems.Add(cartItem);
            }


            await _context.SaveChangesAsync();


            TempData["Success"] =
                "Product added to cart!";


            return RedirectToAction(
                "Index",
                "Products"
            );
        }


        // =========================================
        // REMOVE FROM CART
        // =========================================

        [HttpPost]
        public async Task<IActionResult> Remove(
            int productId)
        {
            int customerId = 1;


            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(
                    ci => ci.ProductID == productId
                      && ci.Cart!.CustomerID == customerId
                      && ci.Cart.Status == "Active"
                );


            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);

                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index");
        }


        // =========================================
        // INCREASE QUANTITY
        // =========================================

        [HttpPost]
        public async Task<IActionResult> Increase(
            int productId)
        {
            int customerId = 1;


            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(
                    ci => ci.ProductID == productId
                      && ci.Cart!.CustomerID == customerId
                      && ci.Cart.Status == "Active"
                );


            if (cartItem != null)
            {
                if (cartItem.Quantity <
                    cartItem.Product!.StockQuantity)
                {
                    cartItem.Quantity++;

                    await _context.SaveChangesAsync();
                }
            }


            return RedirectToAction("Index");
        }


        // =========================================
        // DECREASE QUANTITY
        // =========================================

        [HttpPost]
        public async Task<IActionResult> Decrease(
            int productId)
        {
            int customerId = 1;


            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(
                    ci => ci.ProductID == productId
                      && ci.Cart!.CustomerID == customerId
                      && ci.Cart.Status == "Active"
                );


            if (cartItem != null)
            {
                cartItem.Quantity--;


                if (cartItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }


                await _context.SaveChangesAsync();
            }


            return RedirectToAction("Index");
        }
    }
}