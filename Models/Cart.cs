using System.Collections.Generic;

namespace ShoppingCart.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
