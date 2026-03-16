using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DTOs;
using ShoppingCart.Services;
using System.Security.Claims;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All cart operations require authorization
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> ViewCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartByUserId(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var cart = await _cartService.AddToCart(userId, addToCartDto);

            if (cart == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(cart);
        }

        [HttpPut("update")]
        public async Task<ActionResult<CartDto>> UpdateCartItem([FromBody] UpdateCartItemDto updateCartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var cart = await _cartService.UpdateCartItemQuantity(userId, updateCartItemDto);

            if (cart == null)
            {
                return NotFound("Cart or Cart Item not found.");
            }

            return Ok(cart);
        }

        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            var result = await _cartService.RemoveFromCart(userId, productId);

            if (!result)
            {
                return NotFound("Cart or Cart Item not found.");
            }

            return NoContent();
        }
    }
}
