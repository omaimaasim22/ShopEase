using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("CATEGORY")]
    public class Category
    {
        [Key]
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [Required]
        [Column("CategoryName")]
        public string CategoryName { get; set; } = string.Empty;

        // One Category → Many Products
        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}