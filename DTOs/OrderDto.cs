using System;
using System.Collections.Generic;

namespace ShoppingCart.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    public class CreateOrderDto
    {
        // This DTO might be empty if the order is created directly from the cart
        // or it could contain payment method details.
        // For now, we'll assume it's created from the cart.
        public string PaymentMethod { get; set; } // e.g., "Wallet", "COD"
    }

    public class UpdateOrderStatusDto
    {
        public string NewStatus { get; set; }
    }
}
