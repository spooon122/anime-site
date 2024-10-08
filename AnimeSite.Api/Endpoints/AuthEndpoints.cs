﻿using anime_site.Dto;
using AnimeSite.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System.Security.Claims;

namespace anime_site.Endpoints
{
    /// <summary>
    /// Extension class for defining Auth endpoints.
    /// </summary>
    public static class AuthEndpoints
    {
        /// <summary>
        /// Map out login endpoints.
        /// </summary>
        /// <param name="app"></param>
        public static void LoginEndpoints(this WebApplication app)
        {
            /// <summary>
            /// Group for Auth endpoints
            /// </summary>
            var auth = app.MapGroup("auth");
            /// <summary>
            /// method for login user via Email and Pass
            /// </summary>
            auth.MapPost("/login", async (HttpContext context, [FromBody] LoginRequestDto loginRequest,
                             UserManager<User> userManager,
                             SignInManager<User> signInManager
                             ) =>
            {
                var user = await userManager.FindByEmailAsync(loginRequest.Email!);
                if (user == null)
                {
                    return Results.BadRequest("Пользователя не существует :(");
                }
                
                var result = await signInManager.PasswordSignInAsync(user, loginRequest.Password!, isPersistent: loginRequest.RememberMe, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return Results.BadRequest("Неверный логин или пароль!");
                }

                var response = new
                {
                    userId = user.Id,
                    username = user.UserName
                };

                return Results.Ok(response);
            });

            /// <summary>
            /// method for registration and got confirm code for confirm user email
            /// </summary>
            auth.MapPost("/register", async ([FromBody] RegisterRequestModel model, [FromServices] IEmailService emailSender, UserManager <User> userManager, HttpContext httpContext) =>
            {

                var existingUser = await userManager.FindByEmailAsync(model.Email);

                if (existingUser != null)
                {
                    return Results.BadRequest("Пользователь с этим Email уже существует!");
                }

                var user = new User { UserName = model.Username, Email = model.Email };
                
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/auth/confirm?userId={user.Id}&code={Uri.EscapeDataString(code)}";
                    await emailSender.SendAsync(user.Email, "Send from animeq", callbackUrl);

                    return Results.Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                }

                if (!result.Succeeded)
                {
                    return Results.BadRequest(result.Errors);
                }
                return Results.Ok(result);
                
            });

            /// <summary>
            /// method for confirm user Email
            /// </summary>
            auth.MapGet("confirm", async(string userId, string code, UserManager<User> userManager) =>
            {
                var user = await userManager.FindByIdAsync(userId);
                await userManager.ConfirmEmailAsync(user!, code);
                return Results.Redirect("http://localhost:5173/login");
            });
        }
    }
}
