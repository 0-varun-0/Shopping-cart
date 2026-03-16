namespace ShoppingCart.DTOs
{
    public class UserProfileDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
    }

    public class UpdateUserProfileDto
    {
        public string UserName { get; set; }
        public string Address { get; set; }
    }
}
