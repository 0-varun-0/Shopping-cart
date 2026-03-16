using Microsoft.AspNetCore.Identity;

namespace ShoppingCart.Models
{
    public class User : IdentityUser
    {
        public string? Address { get; set; }
    }
}
