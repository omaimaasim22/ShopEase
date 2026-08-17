using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Pages.Receipt
{
    public class IndexModel : PageModel
    {
        private readonly ShopEaseDbContext _context;

        public IndexModel(ShopEaseDbContext context)
        {
            _context = context;
        }


        // =========================================
        // ORDER
        // =========================================

        public Order? Order { get; set; }


        // =========================================
        // LOAD RECEIPT
        // =========================================

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            Order = await _context.Orders

                // CUSTOMER
                .Include(o => o.Customer)

                // ORDER ITEMS
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)

                // PAYMENT
                .Include(o => o.Payment)

                .FirstOrDefaultAsync(o =>
                    o.OrderID == orderId);


            // -----------------------------------------
            // Order not found
            // -----------------------------------------

            if (Order == null)
            {
                return NotFound();
            }


            return Page();
        }
    }
}