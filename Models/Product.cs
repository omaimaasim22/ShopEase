using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("PRODUCT")]
    public class Product
    {
        [Key]
        [Column("ProductID")]
        public int ProductID { get; set; }

        [Required]
        [Column("ProductName")]
        public string ProductName { get; set; } = string.Empty;

        [Column("Description")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative.")]
        [Column("Price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        [Column("StockQuantity")]
        public int StockQuantity { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [Column("ProductImage")]
        public string? ProductImage { get; set; }

        // Relationship
        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }


        // Relationships with CartItem
        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();


        // Relationships with OrderItem
        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}