using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("CART_ITEM")]
    public class CartItem
    {
        [Column("CartID")]
        public int CartID { get; set; }

        [Column("ProductID")]
        public int ProductID { get; set; }

        [Column("Quantity")]
        public int Quantity { get; set; }


        public Cart? Cart { get; set; }

        public Product? Product { get; set; }
    }
}