using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEase.Models
{
    [Table("PAYMENT")]
    public class Payment
    {
        [Key]
        [Column("PaymentID")]
        public int PaymentID { get; set; }

        [Column("OrderID")]
        public int OrderID { get; set; }

        [Column("PaymentDate")]
        public DateTime PaymentDate { get; set; }

        [Column("PaymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column("Amount", TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Column("PaymentStatus")]
        public string PaymentStatus { get; set; } = "Pending";


        public Order? Order { get; set; }
    }
}