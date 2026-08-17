using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("CART")]
    public class Cart
    {
        [Key]
        [Column("CartID")]
        public int CartID { get; set; }

        [Column("CustomerID")]
        public int? CustomerID { get; set; }

        [Column("CartDate")]
        public DateTime CartDate { get; set; }

        [Column("Status")]
        public string Status { get; set; } = "Active";


        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }


        public ICollection<CartItem> CartItems { get; set; }
            = new List<CartItem>();
    }
}