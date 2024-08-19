using anime_site.Users;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using AnimeSite.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace anime_site.Endpoints
{
    public static class AuthEndpoints
    {
        public static void UserEndpoints(this WebApplication app)
        {
            var auth = app.MapGroup("auth");

            //auth.MapPost("/login", async (LoginUserRequest loginUserRequest, UserManager<User> userManager, SignInManager<User> signInManager) =>
            //{
            //    if (string.IsNullOrEmpty(loginUserRequest.Email) || string.IsNullOrEmpty(loginUserRequest.Password))
            //    {
            //        return Results.BadRequest("Please provide both email and password.");
            //    }

            //    var userEmail = await userManager.FindByEmailAsync(loginUserRequest.Email);

            //    if (userEmail == null)
            //    {
            //        return Results.BadRequest("Invalid email or password.");
            //    }

            //    var result = await signInManager.PasswordSignInAsync(userEmail, loginUserRequest.Password, loginUserRequest.RememberMe, lockoutOnFailure: false);


            //    if (result.Succeeded)
            //    {
            //        return Results.Ok("User logged in successfully.");
            //    }
            //    else if (result.IsLockedOut)
            //    {
            //        return Results.BadRequest("User account locked out.");
            //    }
            //    else if (result.RequiresTwoFactor)
            //    {
            //        return Results.BadRequest("Requires two-factor authentication.");
            //    }
            //    else
            //    {
            //        return Results.BadRequest("Invalid login attempt.");
            //    }
            //});

            auth.MapPost("/login", async (HttpContext context, [FromBody] LoginRequest loginRequest,
                             UserManager<User> userManager,
                             SignInManager<User> signInManager,
                             IJwtTokenService jwtTokenService) =>
            {
                var user = await userManager.FindByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid email or password");
                    return;
                }

                var result = await signInManager.PasswordSignInAsync(user, loginRequest.Password, false, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid email or password");
                    return;
                }

                
                var accessToken = jwtTokenService.GenerateAccessToken(user);
                
                var refreshToken = jwtTokenService.GenerateRefreshToken();
                var response = new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    AccessToken = accessToken,
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            });
            


            auth.MapPost("/registration", async (RegisterUserRequest model, UserManager<User> userManager) =>
            {
                // Валидация модели регистрации
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                {
                    return Results.BadRequest("Please provide all required fields.");
                }
                // Проверка, существует ли пользователь
                var existingUser = await userManager.FindByNameAsync(model.Email);
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

            auth.MapPost("/logout", async (SignInManager<User> signInManager, [FromBody] object empty) =>
            {
                if (empty != null)
                {
                    await signInManager.SignOutAsync();
                    return Results.Ok();
                }
                return Results.Unauthorized();

            });
        }
    }
}
