using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DTOs;
using ShoppingCart.Services;
using System.Security.Claims;

namespace ShoppingCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All profile operations require authorization
    public class ProfileController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = GetUserId();
            try
            {
                var profile = await _profileService.GetUserProfile(userId);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var result = await _profileService.UpdateUserProfile(userId, updateDto);

            if (!result)
            {
                return BadRequest("Failed to update user profile.");
            }

            return NoContent();
        }
    }
}
