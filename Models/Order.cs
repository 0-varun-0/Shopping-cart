using System;
using System.Collections.Generic;

namespace ShoppingCart.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
