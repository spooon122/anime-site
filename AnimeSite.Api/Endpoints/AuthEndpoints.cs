using anime_site.Dto;
using AnimeSite.Core.Models;
using AnimeSite.Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using NETCore.MailKit.Core;
using System.Net.Http;

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

            auth.MapPost("/login", async (HttpContext context, [FromBody] LoginRequestDto loginRequest,
                             UserManager<User> userManager,
                             SignInManager<User> signInManager,
                             IJwtTokenService jwtTokenService) =>
            {
                var user = await userManager.FindByEmailAsync(loginRequest.Email!);
                if (user == null)
                {
                    return Results.BadRequest("Пользователя не существует :(");
                }

                var result = await signInManager.PasswordSignInAsync(user, loginRequest.Password!, loginRequest.RememberMe, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return Results.BadRequest("Неверный логин или пароль!");
                }

                var accessToken = jwtTokenService.GenerateAccessToken(user);
                var refreshToken = jwtTokenService.GenerateRefreshToken();

                context.Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(14)
                });

                var response = new
                {
                    AccessToken = accessToken,
                };

                return Results.Ok(response);
            });

             
            auth.MapPost("/registration", async ([FromBody] RegisterRequestModel model, [FromServices] IEmailService emailSender, UserManager <User> userManager, HttpContext httpContext) =>
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

                    var callbackUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/confirmemail?userId={user.Id}&code={Uri.EscapeDataString(code)}";
                    await emailSender.SendAsync(user.Email, "Send from animeq", callbackUrl);

                    return Results.Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                }
                if (!result.Succeeded)
                {
                    return Results.BadRequest(result.Errors);
                }
                return Results.Ok(result);
                
            });


            auth.MapPost("/refresh", async (HttpContext context,
                                     UserManager<User> userManager,
                                     IJwtTokenService jwtTokenService) =>
            {
                var refreshToken = context.Request.Cookies["RefreshToken"];

                var accessToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                var principal = jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
                var username = principal.Identity?.Name;

                var user = await userManager.FindByNameAsync(username!);

                var newAccessToken = jwtTokenService.GenerateAccessToken(user);
                var newRefreshToken = jwtTokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                await userManager.UpdateAsync(user);

                context.Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
                
                return Results.Ok(new { AccessToken = newAccessToken });
            });

            auth.MapGet("confirm", async(string userId, string code, UserManager<User> userManager) =>
            {
                var user = await userManager.FindByNameAsync(userId);
                await userManager.ConfirmEmailAsync(user!, code);
            });
        }
    }
}
