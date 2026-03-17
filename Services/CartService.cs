using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Models;
using ShoppingCart.DTOs;
using Microsoft.AspNetCore.Identity; // Added this

namespace ShoppingCart.Services
{
    public class CartService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager; // Added this

        public CartService(AppDbContext context, UserManager<User> userManager) // Modified constructor
        {
            _context = context;
            _userManager = userManager; // Initialized userManager
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Create a new cart if one doesn't exist for the user
                var user = await _userManager.FindByIdAsync(userId); // Fetch the user
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {userId} not found."); // Or handle appropriately
                }

                cart = new Cart { UserId = userId, User = user, TotalPrice = 0, CartItems = new List<CartItem>() }; // Assign the user
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                TotalPrice = cart.TotalPrice,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    Total = ci.Quantity * ci.Product.Price
                }).ToList()
            };
        }

        public async Task<CartDto?> AddToCart(string userId, AddToCartDto addToCartDto)
        {
            var product = await _context.Products.FindAsync(addToCartDto.ProductId);
            if (product == null)
            {
                return null; // Product not found
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, TotalPrice = 0, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == addToCartDto.ProductId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += addToCartDto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    Product = product // Attach product for easier DTO mapping
                });
            }

            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
            await _context.SaveChangesAsync();

            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                TotalPrice = cart.TotalPrice,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    Total = ci.Quantity * ci.Product.Price
                }).ToList()
            };
        }

        public async Task<CartDto?> UpdateCartItemQuantity(string userId, UpdateCartItemDto updateCartItemDto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return null; // Cart not found
            }

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == updateCartItemDto.ProductId);

            if (existingCartItem == null)
            {
                return null; // Cart item not found
            }

            existingCartItem.Quantity = updateCartItemDto.Quantity;
            if (existingCartItem.Quantity <= 0)
            {
                cart.CartItems.Remove(existingCartItem);
            }

            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);
            await _context.SaveChangesAsync();

            return new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                TotalPrice = cart.TotalPrice,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price,
                    Total = ci.Quantity * ci.Product.Price
                }).ToList()
            };
        }

        public async Task<bool> RemoveFromCart(string userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return false; // Cart not found
            }

            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingCartItem == null)
            {
                return false; // Cart item not found
            }

            _context.CartItems.Remove(existingCartItem);
            cart.TotalPrice = cart.CartItems.Where(ci => ci.ProductId != productId).Sum(ci => ci.Quantity * ci.Product.Price); // Recalculate total
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
