using Microsoft.EntityFrameworkCore;
using ShopEase.Models;

namespace ShopEase.Data
{
    public class ShopEaseDbContext : DbContext
    {
        public ShopEaseDbContext(
            DbContextOptions<ShopEaseDbContext> options)
            : base(options)
        {
        }


        public DbSet<Customer> Customers { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<InventoryTransaction>
            InventoryTransactions
        { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =========================================
            // CART_ITEM COMPOSITE PRIMARY KEY
            // =========================================

            modelBuilder.Entity<CartItem>()
                .HasKey(x => new
                {
                    x.CartID,
                    x.ProductID
                });


            // =========================================
            // ORDER_ITEM COMPOSITE PRIMARY KEY
            // =========================================

            modelBuilder.Entity<OrderItem>()
                .HasKey(x => new
                {
                    x.OrderID,
                    x.ProductID
                });


            // =========================================
            // CATEGORY → PRODUCT
            // =========================================

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID);


            // =========================================
            // CUSTOMER → CART
            // =========================================

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithMany(c => c.Carts)
                .HasForeignKey(c => c.CustomerID);


            // =========================================
            // CART → CART_ITEM
            // =========================================

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(c => c.CartID);


            // =========================================
            // PRODUCT → CART_ITEM
            // =========================================

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(c => c.ProductID);


            // =========================================
            // CUSTOMER → ORDER
            // =========================================

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerID);


            // =========================================
            // ORDER → ORDER_ITEM
            // =========================================

            modelBuilder.Entity<OrderItem>()
                .HasOne(i => i.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(i => i.OrderID);


            // =========================================
            // PRODUCT → ORDER_ITEM
            // =========================================

            modelBuilder.Entity<OrderItem>()
                .HasOne(i => i.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(i => i.ProductID);


            // =========================================
            // ORDER → PAYMENT
            // =========================================

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderID);


            // =========================================
            // PRODUCT → INVENTORY TRANSACTION
            // =========================================

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductID);


            // =========================================
            // ORDER → INVENTORY TRANSACTION
            // =========================================

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(i => i.Order)
                .WithMany(o => o.InventoryTransactions)
                .HasForeignKey(i => i.OrderID);
        }
    }
}