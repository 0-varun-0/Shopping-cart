namespace ShoppingCart.DTOs
{
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public class CartDto
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }

    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
