using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShopEase.Data;
using ShopEase.Models;

namespace ShopEase.Pages.Checkout
{
    public class IndexModel : PageModel
    {
        private readonly ShopEaseDbContext _context;

        public IndexModel(ShopEaseDbContext context)
        {
            _context = context;
        }


        // =========================================
        // CUSTOMER INFORMATION
        // =========================================

        [BindProperty]
        public string FullName { get; set; } = "";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Phone { get; set; } = "";

        [BindProperty]
        public string Address { get; set; } = "";


        // =========================================
        // CART INFORMATION
        // =========================================

        public List<CartItem> CartItems { get; set; } = new();

        public decimal Total { get; set; }


        // =========================================
        // LOAD CHECKOUT PAGE
        // =========================================

        public async Task<IActionResult> OnGetAsync()
        {
            int? cartId =
                HttpContext.Session.GetInt32("CartID");

            if (!cartId.HasValue)
            {
                return RedirectToPage("/Products/Index");
            }


            CartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p!.Category)
                .Where(c => c.CartID == cartId.Value)
                .ToListAsync();


            if (!CartItems.Any())
            {
                return RedirectToPage("/Cart/Index");
            }


            Total = CartItems.Sum(item =>
                item.Product != null
                    ? item.Quantity * item.Product.Price
                    : 0
            );


            return Page();
        }


        // =========================================
        // CONFIRM ORDER
        // =========================================

        public async Task<IActionResult> OnPostConfirmOrderAsync()
        {
            // -----------------------------------------
            // 1. Validate customer information
            // -----------------------------------------

            if (string.IsNullOrWhiteSpace(FullName))
            {
                ModelState.AddModelError(
                    nameof(FullName),
                    "Please enter your full name."
                );
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(
                    nameof(Email),
                    "Please enter your email."
                );
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                ModelState.AddModelError(
                    nameof(Phone),
                    "Please enter your phone number."
                );
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                ModelState.AddModelError(
                    nameof(Address),
                    "Please enter your address."
                );
            }


            if (!ModelState.IsValid)
            {
                await LoadCart();

                return Page();
            }


            // -----------------------------------------
            // 2. Get CartID from session
            // -----------------------------------------

            int? cartId =
                HttpContext.Session.GetInt32("CartID");


            if (!cartId.HasValue)
            {
                return RedirectToPage("/Cart/Index");
            }


            // -----------------------------------------
            // 3. Load cart
            // -----------------------------------------

            var cart = await _context.Carts
                .FirstOrDefaultAsync(c =>
                    c.CartID == cartId.Value &&
                    c.Status == "Active");


            if (cart == null)
            {
                return RedirectToPage("/Cart/Index");
            }


            // -----------------------------------------
            // 4. Load cart items
            // -----------------------------------------

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartID == cart.CartID)
                .ToListAsync();


            if (!cartItems.Any())
            {
                return RedirectToPage("/Cart/Index");
            }


            // -----------------------------------------
            // 5. Start database transaction
            // -----------------------------------------

            using var transaction =
                await _context.Database.BeginTransactionAsync();


            try
            {
                // -----------------------------------------
                // 6. Find existing customer by email
                // -----------------------------------------

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c =>
                        c.Email == Email);


                // -----------------------------------------
                // 7. Create customer if new
                // -----------------------------------------

                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = FullName,
                        Email = Email,
                        Phone = Phone,
                        Address = Address
                      
                    };

                    _context.Customers.Add(customer);

                    await _context.SaveChangesAsync();
                }


                // -----------------------------------------
                // 8. Update customer information
                // -----------------------------------------

                else
                {
                    customer.FullName = FullName;
                    customer.Phone = Phone;
                    customer.Address = Address;

                    await _context.SaveChangesAsync();
                }


                // -----------------------------------------
                // 9. Attach customer to cart
                // -----------------------------------------

                cart.CustomerID = customer.CustomerID;


                // -----------------------------------------
                // 10. Calculate total
                // -----------------------------------------

                decimal totalAmount = 0;


                foreach (var item in cartItems)
                {
                    if (item.Product == null)
                    {
                        throw new Exception(
                            "A product in the cart could not be found."
                        );
                    }


                    // Check stock BEFORE creating order

                    if (item.Quantity >
                        item.Product.StockQuantity)
                    {
                        throw new Exception(
                            $"Not enough stock available for {item.Product.ProductName}."
                        );
                    }


                    totalAmount +=
                        item.Quantity *
                        item.Product.Price;
                }


                // -----------------------------------------
                // 11. Create order
                // -----------------------------------------

                var order = new Order
                {
                    CustomerID = customer.CustomerID,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    OrderStatus = "Pending"
                };


                _context.Orders.Add(order);

                await _context.SaveChangesAsync();


                // -----------------------------------------
                // 12. Copy CART_ITEM → ORDER_ITEM
                // -----------------------------------------

                foreach (var item in cartItems)
                {
                    if (item.Product == null)
                    {
                        continue;
                    }


                    var orderItem = new OrderItem
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    };


                    _context.OrderItems.Add(orderItem);
                }


                await _context.SaveChangesAsync();


                // -----------------------------------------
                // 13. CONFIRM ORDER
                // -----------------------------------------
                //
                // This is VERY IMPORTANT.
                //
                // Changing Pending → Confirmed
                // activates our MySQL trigger.
                //
                // The trigger:
                //
                // PRODUCT.StockQuantity
                //       ↓
                // decreases
                //
                // and creates:
                //
                // INVENTORY_TRANSACTION
                //

                order.OrderStatus = "Confirmed";

                await _context.SaveChangesAsync();


                // -----------------------------------------
                // 14. Create payment record
                // -----------------------------------------

                var payment = new Payment
                {
                    OrderID = order.OrderID,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = "Cash on Delivery",
                    Amount = totalAmount,
                    PaymentStatus = "Pending"
                };


                _context.Payments.Add(payment);


                // -----------------------------------------
                // 15. Mark cart as checked out
                // -----------------------------------------

                cart.Status = "CheckedOut";


                await _context.SaveChangesAsync();


                // -----------------------------------------
                // 16. Commit everything
                // -----------------------------------------

                await transaction.CommitAsync();


                // -----------------------------------------
                // 17. Remove CartID from session
                // -----------------------------------------

                HttpContext.Session.Remove("CartID");


                // -----------------------------------------
                // 18. Go to receipt
                // -----------------------------------------

                return RedirectToPage(
                    "/Receipt/Index",
                    new
                    {
                        orderId = order.OrderID
                    }
                );
            }
            catch (Exception ex)
            {
                // -----------------------------------------
                // Something went wrong.
                //
                // Roll EVERYTHING back.
                // -----------------------------------------

                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    "",
                    ex.Message
                );

                await LoadCart();

                return Page();
            }
        }


        // =========================================
        // LOAD CART
        // =========================================

        private async Task LoadCart()
        {
            int? cartId =
                HttpContext.Session.GetInt32("CartID");


            if (!cartId.HasValue)
            {
                CartItems = new();
                Total = 0;
                return;
            }


            CartItems = await _context.CartItems
                .Include(c => c.Product)
                .ThenInclude(p => p!.Category)
                .Where(c => c.CartID == cartId.Value)
                .ToListAsync();


            Total = CartItems.Sum(item =>
                item.Product != null
                    ? item.Quantity * item.Product.Price
                    : 0
            );
        }
    }
}