using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Pages.AdminProducts
{
    public class DeleteModel : PageModel
    {
        private readonly ShopEaseDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(
            ShopEaseDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public Product Product { get; set; } = new Product();


        // =========================================
        // GET - SHOW CONFIRMATION
        // =========================================

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (Product == null)
            {
                return NotFound();
            }

            return Page();
        }


        // =========================================
        // POST - DELETE PRODUCT
        // =========================================

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }


            // =========================================
            // DELETE PRODUCT IMAGE
            // =========================================

            if (!string.IsNullOrEmpty(product.ProductImage))
            {
                string imagePath = Path.Combine(
                    _environment.WebRootPath,
                    product.ProductImage.TrimStart('/')
                        .Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }


            // =========================================
            // DELETE PRODUCT FROM DATABASE
            // =========================================

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();


            return RedirectToPage("/AdminProducts/Index");
        }
    }
}