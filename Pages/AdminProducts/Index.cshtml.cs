using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Pages.AdminProducts
{
    public class IndexModel : PageModel
    {
        private readonly ShopEaseDbContext _context;

        public IndexModel(ShopEaseDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; } = new List<Product>();

        public async Task OnGetAsync()
        {
            Products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.ProductID)
                .ToListAsync();
        }
    }
}