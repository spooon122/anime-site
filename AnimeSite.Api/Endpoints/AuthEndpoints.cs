using anime_site.Dto;
using AnimeSite.Core.Models;
using AnimeSite.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace anime_site.Endpoints
{
    public static class AuthEndpoints
    {
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
                
                var refreshToken = jwtTokenService.GenerateAccessToken(user);

                context.Response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict, 
                    Expires = DateTime.UtcNow.AddDays(7) 
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
            

            auth.MapPost("/registration", async (RegisterUserDto registerUserDto, UserManager<User> userManager) =>
            {
                
                if (string.IsNullOrEmpty(registerUserDto.Username) || string.IsNullOrEmpty(registerUserDto.Email) || string.IsNullOrEmpty(registerUserDto.Password))
                {
                    return Results.BadRequest("Please provide all required fields.");
                }
                
                var existingUser = await userManager.FindByNameAsync(registerUserDto.Email);
                if (existingUser != null)
                {
                    return Results.BadRequest("Username is already taken.");
                }

                var user = new User
                {
                    UserName = registerUserDto.Username,
                    Email = registerUserDto.Email
                };

                
                var result = await userManager.CreateAsync(user, registerUserDto.Password);
                if (result.Succeeded)
                {
                    return Results.Ok("User registered successfully.");
                }
                else
                {
                    return Results.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            });


            auth.MapPost("/refresh", async (HttpContext context,
                                     [FromBody] RefreshRequest refreshRequest,
                                     UserManager<User> userManager,
                                     IJwtTokenService jwtTokenService) =>
            {

                var principal = jwtTokenService.GetPrincipalFromExpiredToken(refreshRequest.RefreshToken);
                
                if (principal == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid refresh token");
                    return;
                }

                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await userManager.FindByIdAsync(userId);


                var newAccessToken = jwtTokenService.GenerateAccessToken(user);
                var newRefreshToken = refreshRequest.RefreshToken;

                context.Response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                var response = new
                {
                    RefreshToken = newRefreshToken,
                    AccessToken = newAccessToken
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
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
