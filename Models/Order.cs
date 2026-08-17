using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("ORDERS")]
    public class Order
    {
        [Key]
        [Column("OrderID")]
        public int OrderID { get; set; }

        [Column("CustomerID")]
        public int CustomerID { get; set; }

        [Column("OrderDate")]
        public DateTime OrderDate { get; set; }

        [Column("TotalAmount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Column("OrderStatus")]
        public string OrderStatus { get; set; } = "Pending";


        public Customer? Customer { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();

        public Payment? Payment { get; set; }

        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
            = new List<InventoryTransaction>();
    }
}