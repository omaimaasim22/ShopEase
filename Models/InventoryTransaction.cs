using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("INVENTORY_TRANSACTION")]
    public class InventoryTransaction
    {
        [Key]
        [Column("InventoryTransactionID")]
        public int InventoryTransactionID { get; set; }

        [Column("ProductID")]
        public int ProductID { get; set; }

        [Column("OrderID")]
        public int? OrderID { get; set; }

        [Column("QuantityChange")]
        public int QuantityChange { get; set; }

        [Column("TransactionType")]
        public string TransactionType { get; set; } = string.Empty;

        [Column("TransactionDate")]
        public DateTime TransactionDate { get; set; }


        public Product? Product { get; set; }

        public Order? Order { get; set; }
    }
}