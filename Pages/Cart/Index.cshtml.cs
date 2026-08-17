using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly ShopEaseDbContext _context;

        public IndexModel(ShopEaseDbContext context)
        {
            _context = context;
        }

        public List<CartItem> CartItems { get; set; } = new();

        public decimal Total { get; set; }


        // =========================================
        // LOAD CART
        // =========================================

        public async Task OnGetAsync()
        {
            await LoadCart();
        }


        // =========================================
        // INCREASE QUANTITY
        // =========================================

        public async Task<IActionResult> OnPostIncreaseAsync(int productId)
        {
            int? cartId = HttpContext.Session.GetInt32("CartID");

            if (!cartId.HasValue)
            {
                return RedirectToPage();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c =>
                    c.CartID == cartId.Value &&
                    c.ProductID == productId);

            if (cartItem == null)
            {
                return RedirectToPage();
            }

            // Do not exceed available stock
            if (cartItem.Product != null &&
                cartItem.Quantity < cartItem.Product.StockQuantity)
            {
                cartItem.Quantity++;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        // =========================================
        // DECREASE QUANTITY
        // =========================================

        public async Task<IActionResult> OnPostDecreaseAsync(int productId)
        {
            int? cartId = HttpContext.Session.GetInt32("CartID");

            if (!cartId.HasValue)
            {
                return RedirectToPage();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CartID == cartId.Value &&
                    c.ProductID == productId);

            if (cartItem == null)
            {
                return RedirectToPage();
            }

            // Minimum quantity is 1
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        // =========================================
        // REMOVE ITEM
        // =========================================

        public async Task<IActionResult> OnPostRemoveAsync(int productId)
        {
            int? cartId = HttpContext.Session.GetInt32("CartID");

            if (!cartId.HasValue)
            {
                return RedirectToPage();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c =>
                    c.CartID == cartId.Value &&
                    c.ProductID == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }


        // =========================================
        // LOAD CART FROM DATABASE
        // =========================================

        private async Task LoadCart()
        {
            int? cartId = HttpContext.Session.GetInt32("CartID");

            if (!cartId.HasValue)
            {
                CartItems = new List<CartItem>();
                Total = 0;
                return;
            }

            CartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p!.Category)
                .Where(c => c.CartID == cartId.Value)
                .ToListAsync();

            Total = CartItems.Sum(item =>
                item.Product != null
                    ? item.Quantity * item.Product.Price
                    : 0);
        }
    }
}