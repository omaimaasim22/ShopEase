using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("CUSTOMER")]
    public class Customer
    {
        [Key]
        [Column("CustomerID")]
        public int CustomerID { get; set; }

        [Required]
        [Column("FullName")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        [Column("Phone")]
        public string? Phone { get; set; }

        [Column("Address")]
        public string? Address { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }


        public ICollection<Cart> Carts { get; set; }
            = new List<Cart>();

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
    }
}