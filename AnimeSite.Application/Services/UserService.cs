using AnimeSite.Core.Interfaces;
using AnimeSite.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace AnimeSite.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task UpdateUserNameAsync(string userId, string newUserName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Пользователь не найден!");
            }

            user.UserName = newUserName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Ошибка при изменеие имени пользователя: " +
                    $"Ошибка {errors}");
            }
        }
    }
}
