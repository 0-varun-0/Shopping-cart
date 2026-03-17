using Microsoft.AspNetCore.Identity;
using ShoppingCart.Models;
using ShoppingCart.DTOs;

namespace ShoppingCart.Services
{
    public class ProfileService
    {
        private readonly UserManager<User> _userManager;

        public ProfileService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserProfileDto?> GetUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID '{userId}' not found in UserManager.");
            }

            return new UserProfileDto
            {
                Email = user.Email,
                UserName = user.UserName,
                Address = user.Address
            };
        }

        public async Task<bool> UpdateUserProfile(string userId, UpdateUserProfileDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.UserName = updateDto.UserName;
            user.Address = updateDto.Address;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
