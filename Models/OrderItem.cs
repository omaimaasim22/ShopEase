using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("ORDER_ITEM")]
    public class OrderItem
    {
        [Column("OrderID")]
        public int OrderID { get; set; }

        [Column("ProductID")]
        public int ProductID { get; set; }

        [Column("Quantity")]
        public int Quantity { get; set; }

        [Column("UnitPrice", TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }


        public Order? Order { get; set; }

        public Product? Product { get; set; }
    }
}