using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Pages.AdminProducts
{
    public class EditModel : PageModel
    {
        private readonly ShopEaseDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(
            ShopEaseDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Product Product { get; set; } = new Product();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public List<Category> Categories { get; set; } = new List<Category>();


        // =========================================
        // GET
        // =========================================

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (Product == null)
            {
                return NotFound();
            }

            Categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return Page();
        }


        // =========================================
        // POST - UPDATE PRODUCT
        // =========================================

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Remove navigation property validation
            ModelState.Remove("Product.Category");

            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();

                return Page();
            }

            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (existingProduct == null)
            {
                return NotFound();
            }


            // =========================================
            // UPDATE PRODUCT INFORMATION
            // =========================================

            existingProduct.ProductName = Product.ProductName;
            existingProduct.Description = Product.Description;
            existingProduct.Price = Product.Price;
            existingProduct.StockQuantity = Product.StockQuantity;
            existingProduct.CategoryID = Product.CategoryID;


            // =========================================
            // UPDATE IMAGE IF NEW IMAGE WAS SELECTED
            // =========================================

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "products"
                );

                Directory.CreateDirectory(uploadsFolder);


                // Delete old image if it exists
                if (!string.IsNullOrEmpty(existingProduct.ProductImage))
                {
                    string oldImagePath = Path.Combine(
                        _environment.WebRootPath,
                        existingProduct.ProductImage.TrimStart('/')
                            .Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }


                // Create new file name
                string extension = Path.GetExtension(ImageFile.FileName);

                string fileName = Guid.NewGuid().ToString()
                    + extension;

                string filePath = Path.Combine(
                    uploadsFolder,
                    fileName
                );


                // Save new image
                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }


                // Save path in database
                existingProduct.ProductImage =
                    "/uploads/products/" + fileName;
            }


            // =========================================
            // SAVE CHANGES TO DATABASE
            // =========================================

            await _context.SaveChangesAsync();


            // Return to Admin Products page
            return RedirectToPage("/AdminProducts/Index");
        }
    }
}