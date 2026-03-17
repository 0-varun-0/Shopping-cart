using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DTOs;
using ShoppingCart.Services;
using System.Security.Claims;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All order operations require authorization
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<OrderDto>> PlaceOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var order = await _orderService.PlaceOrder(userId, createOrderDto);

            if (order == null)
            {
                return BadRequest("Could not place order. Cart might be empty, product stock insufficient, or payment failed.");
            }

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, order);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrderHistory()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetOrderHistory(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int orderId)
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderById(userId, orderId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPut("{orderId}/status")]
        // Potentially add role-based authorization here, e.g., [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto updateStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _orderService.UpdateOrderStatus(orderId, updateStatusDto.NewStatus);

            if (!result)
            {
                return BadRequest("Failed to update order status. Order not found or invalid status.");
            }

            return NoContent();
        }
    }
}
