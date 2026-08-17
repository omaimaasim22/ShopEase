using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Pages.AdminProducts
{
    public class CreateModel : PageModel
    {
        private readonly ShopEaseDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(
            ShopEaseDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // =========================================
        // PRODUCT FORM
        // =========================================

        [BindProperty]
        public Product Product { get; set; } = new Product();


        // =========================================
        // IMAGE
        // =========================================

        [BindProperty]
        public IFormFile? ImageFile { get; set; }


        // =========================================
        // CATEGORIES
        // =========================================

        public IList<Category> Categories { get; set; }
            = new List<Category>();


        // =========================================
        // SHOW FORM
        // =========================================

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }


        // =========================================
        // INSERT PRODUCT
        // =========================================

        public async Task<IActionResult> OnPostAsync()
        {
            // Navigation property is not submitted
            ModelState.Remove("Product.Category");


            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();

                return Page();
            }


            // =========================================
            // SAVE IMAGE
            // =========================================

            if (ImageFile != null && ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "images",
                    "products"
                );

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }


                string extension =
                    Path.GetExtension(ImageFile.FileName);


                string fileName =
                    Guid.NewGuid().ToString() + extension;


                string filePath =
                    Path.Combine(uploadsFolder, fileName);


                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }


                Product.ProductImage =
                    "/images/products/" + fileName;
            }


            // =========================================
            // INSERT INTO MYSQL
            // =========================================

            _context.Products.Add(Product);

            await _context.SaveChangesAsync();


            // =========================================
            // RETURN TO ADMIN PRODUCTS
            // =========================================

            TempData["Success"] =
                "Product added successfully.";

            return RedirectToPage("/AdminProducts/Index");
        }
    }
}