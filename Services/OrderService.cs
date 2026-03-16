using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Models;
using ShoppingCart.DTOs;

namespace ShoppingCart.Services
{
    public class OrderService
    {
        private readonly AppDbContext _context;
        private readonly CartService _cartService;
        private readonly WalletService _walletService; // Injected WalletService

        public OrderService(AppDbContext context, CartService cartService, WalletService walletService) // Modified constructor
        {
            _context = context;
            _cartService = cartService;
            _walletService = walletService; // Initialized WalletService
        }

        public async Task<OrderDto?> PlaceOrder(string userId, CreateOrderDto createOrderDto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return null; // Cart is empty or not found
            }

            // Check product stock before placing order
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null || product.Stock < cartItem.Quantity)
                {
                    // Handle insufficient stock or product not found
                    return null; // Or throw a specific exception
                }
            }

            // Process Payment if method is Wallet
            string orderStatus = "Pending";
            if (createOrderDto.PaymentMethod == "Wallet")
            {
                bool paymentSuccessful = await _walletService.ProcessPayment(userId, cart.TotalPrice);
                if (!paymentSuccessful)
                {
                    return null; // Payment failed (e.g., insufficient balance)
                }
                orderStatus = "Successful"; // Set to successful if wallet payment goes through
            }
            else if (createOrderDto.PaymentMethod == "COD")
            {
                orderStatus = "Pending"; // COD orders are pending until delivery
            }
            else
            {
                return null; // Invalid payment method
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = orderStatus, // Set status based on payment
                TotalAmount = cart.TotalPrice,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price // Price at the time of order
                }).ToList()
            };

            _context.Orders.Add(order);

            // Deduct stock
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(orderItem.ProductId);
                if (product != null)
                {
                    product.Stock -= orderItem.Quantity;
                }
            }

            // Clear the cart after placing the order
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Carts.Remove(cart); // Remove the cart itself
            await _context.SaveChangesAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = _context.Products.Find(oi.ProductId)?.Name ?? "Unknown", // Fetch product name
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrderDto>> GetOrderHistory(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Price
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDto?> GetOrderById(string userId, int orderId)
        {
            var order = await _context.Orders
                .Where(o => o.UserId == userId && o.OrderId == orderId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            return new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            // Basic validation for status transition (can be made more robust)
            if (newStatus == "Successful" || newStatus == "Cancelled" || newStatus == "Shipped")
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
                return true;
            }
            return false; // Invalid status
        }
    }
}
