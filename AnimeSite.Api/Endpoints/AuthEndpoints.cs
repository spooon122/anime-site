using anime_site.Users;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace anime_site.Endpoints
{
    public static class AuthEndpoints
    {
        public static void UserEndpoints(this WebApplication app)
        {
            var auth = app.MapGroup("auth");

            auth.MapPost("/login", async (LoginUserRequest loginUserRequest, SignInManager<User> signInManager) =>
            {
                if (string.IsNullOrEmpty(loginUserRequest.UserName) || string.IsNullOrEmpty(loginUserRequest.Password))
                {
                    return Results.BadRequest("Invalid login request.");
                }

                var result = await signInManager.PasswordSignInAsync(loginUserRequest.UserName, loginUserRequest.Password, loginUserRequest.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Results.Ok("User logged in successfully.");
                }
                else if (result.IsLockedOut)
                {
                    return Results.BadRequest("User account locked out.");
                }
                else if (result.RequiresTwoFactor)
                {
                    return Results.BadRequest("Requires two-factor authentication.");
                }
                else
                {
                    return Results.BadRequest("Invalid login attempt.");
                }
            });

            auth.MapPost("/register", async (RegisterUserRequest model, UserManager<User> userManager) =>
            {
                // Валидация модели регистрации
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return Results.BadRequest("Please provide all required fields.");
                }
                // Проверка, существует ли пользователь
                var existingUser = await userManager.FindByNameAsync(model.Username);
                if (existingUser != null)
                {
                    return Results.BadRequest("Username is already taken.");
                }

                var user = new User
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                // Создание пользователя
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Results.Ok("User registered successfully.");
                }
                else
                {
                    return Results.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            });
        }
    }
}
