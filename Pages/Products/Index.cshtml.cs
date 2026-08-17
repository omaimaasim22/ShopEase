using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

// IMPORTANT:
// There is a namespace called ShopEase.Pages.Cart
// and a model called ShopEase.Models.Cart.
// This alias removes the naming conflict.
using CartModel = ShopEase.Models.Cart;

namespace ShopEase.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ShopEaseDbContext _context;

        public IndexModel(ShopEaseDbContext context)
        {
            _context = context;
        }


        // =========================================
        // PRODUCTS
        // =========================================

        public List<Product> Products { get; set; } = new();


        // =========================================
        // CART QUANTITIES
        // =========================================

        public Dictionary<int, int> CartQuantities { get; set; } = new();


        // =========================================
        // DISPLAY PRODUCTS
        // =========================================

        public async Task OnGetAsync()
        {
            // Load all products
            Products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.ProductID)
                .ToListAsync();


            // Get the current cart from browser session
            int? cartId =
                HttpContext.Session.GetInt32("CartID");


            // No cart yet
            if (!cartId.HasValue)
            {
                return;
            }


            // Load quantities already stored
            // in CART_ITEM for this cart
            CartQuantities = await _context.CartItems
                .Where(c => c.CartID == cartId.Value)
                .ToDictionaryAsync(
                    c => c.ProductID,
                    c => c.Quantity
                );
        }


        // =========================================
        // ADD TO CART
        // =========================================

        public async Task<IActionResult> OnPostAddToCartAsync(
            int productId,
            int quantity)
        {
            // -----------------------------------------
            // 1. Validate quantity
            // -----------------------------------------

            if (quantity < 1)
            {
                quantity = 1;
            }


            // -----------------------------------------
            // 2. Find product
            // -----------------------------------------

            var product = await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductID == productId);


            if (product == null)
            {
                return NotFound();
            }


            // -----------------------------------------
            // 3. Check stock
            // -----------------------------------------

            if (product.StockQuantity <= 0)
            {
                return RedirectToPage();
            }


            // -----------------------------------------
            // 4. Don't allow quantity
            //    greater than stock
            // -----------------------------------------

            if (quantity > product.StockQuantity)
            {
                quantity = product.StockQuantity;
            }


            // -----------------------------------------
            // 5. Get CartID from session
            // -----------------------------------------

            int? cartId =
                HttpContext.Session.GetInt32("CartID");


            // IMPORTANT:
            // Explicitly use ShopEase.Models.Cart
            // through the CartModel alias.

            CartModel? cart = null;


            // -----------------------------------------
            // 6. Find existing active cart
            // -----------------------------------------

            if (cartId.HasValue)
            {
                cart = await _context.Carts
                    .FirstOrDefaultAsync(c =>
                        c.CartID == cartId.Value &&
                        c.Status == "Active");
            }


            // -----------------------------------------
            // 7. Create cart if necessary
            // -----------------------------------------

            if (cart == null)
            {
                cart = new CartModel
                {
                    CustomerID = null,
                    CartDate = DateTime.Now,
                    Status = "Active"
                };

                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();


                // Store CartID in browser session
                HttpContext.Session.SetInt32(
                    "CartID",
                    cart.CartID
                );
            }


            // -----------------------------------------
            // 8. Find existing CartItem
            // -----------------------------------------

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci =>
                    ci.CartID == cart.CartID &&
                    ci.ProductID == productId);


            // -----------------------------------------
            // 9. Product NOT already in cart
            // -----------------------------------------

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity
                };

                _context.CartItems.Add(cartItem);
            }


            // -----------------------------------------
            // 10. Product ALREADY in cart
            // -----------------------------------------

            else
            {
                // IMPORTANT:
                //
                // We are NOT adding the old quantity
                // to the new quantity.
                //
                // We simply save the quantity selected
                // by the customer on the Products page.

                cartItem.Quantity = quantity;
            }


            // -----------------------------------------
            // 11. Save to database
            // -----------------------------------------

            await _context.SaveChangesAsync();


            // -----------------------------------------
            // 12. Return to Products page
            // -----------------------------------------

            return RedirectToPage();
        }
    }
}