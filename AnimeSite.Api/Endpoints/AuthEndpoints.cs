﻿using anime_site.Dto;
using AnimeSite.Core.Interfaces;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using AnimeSite.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace anime_site.Endpoints
{
    /// <summary>
    /// Extension class for defining User endpoints.
    /// </summary>
    public static class AuthEndpoints
    {
        /// <summary>
        /// Map out user-related endpoints.
        /// </summary>
        /// <param name="app"></param>
        public static void UserEndpoints(this WebApplication app)
        {

            var auth = app.MapGroup("auth");

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

                var result = await signInManager.PasswordSignInAsync(user, loginRequest.Password, true, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid email or password");
                    return;
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
                    user.Id,
                    Username = user.UserName,
                    AccessToken = accessToken,
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            });


            auth.MapPost("/registration", async ([FromBody] RegisterRequestModel model, UserManager<User> userManager) =>
            {

                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Results.Ok(new { Message = "User registered successfully" });
                }
                return Results.BadRequest(result.Errors);
            });


            auth.MapPost("/refresh", async (HttpContext context,
                                     [FromBody] RefreshTokenRequest refreshTokenRequest,
                                     UserManager<User> userManager,
                                     IJwtTokenService jwtTokenService) =>
            {
                var refreshToken = context.Request.Cookies["RefreshToken"];

                var accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoic3RyaW5nQG1haWwuY29tIiwianRpIjoiNmQ3NzIwZmQtMDI3Ny00MzVhLTgyYTUtOGIxYTI3NjU0MmQ3IiwiZXhwIjoxNzI0MTg1MjA0fQ.9Cv2WKuNLr9JKCaiXZhnjEtHhwEnyktEDDspSB4Ft_4";

                var principal = jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
                var username = principal.Identity?.Name;

                var user = await userManager.FindByNameAsync(username!);
                if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return Results.Unauthorized();
                }

                var newAccessToken = jwtTokenService.GenerateAccessToken(user);
                var newRefreshToken = jwtTokenService.GenerateRefreshToken();

                user.RefreshToken = newRefreshToken;
                await userManager.UpdateAsync(user);

                // Добавление нового refreshToken в cookies
                context.Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Использовать только для HTTPS
                    SameSite = SameSiteMode.Strict
                });

                return Results.Ok(new { AccessToken = accessToken });
            });
        }
    }
}
