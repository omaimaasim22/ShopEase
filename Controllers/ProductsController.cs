using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;

namespace ShopEase.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopEaseDbContext _context;

        public ProductsController(ShopEaseDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }
    }
}