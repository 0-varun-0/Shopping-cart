using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DTOs;
using ShoppingCart.Services;
using System.Security.Claims;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All wallet operations require authorization
    public class WalletController : ControllerBase
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<WalletDto>> GetWallet()
        {
            var userId = GetUserId();
            var wallet = await _walletService.GetWalletByUserId(userId);
            return Ok(wallet);
        }

        [HttpPost("add-funds")]
        public async Task<ActionResult<WalletDto>> AddFunds([FromBody] AddFundsDto addFundsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var wallet = await _walletService.AddFunds(userId, addFundsDto);

            if (wallet == null)
            {
                return NotFound("Wallet not found.");
            }

            return Ok(wallet);
        }

        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto processPaymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var result = await _walletService.ProcessPayment(userId, processPaymentDto.Amount);

            if (!result)
            {
                return BadRequest("Payment failed. Insufficient balance or wallet not found.");
            }

            return Ok("Payment processed successfully.");
        }
    }
}
