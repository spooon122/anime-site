using anime_site.Dto;
using AnimeSite.Core.Interfaces;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



namespace anime_site.Endpoints
{
    public static class AuthEndpoints
    {
        public static void UserEndpoints(this WebApplication app)
        {
            var auth = app.MapGroup("auth");
            var users = app.MapGroup("users").RequireAuthorization();

            //auth.MapPost("/registration", async (RegisterUserDto registerUserDto, UserManager <User> userManager) =>
            //{

            //    if (string.IsNullOrEmpty(registerUserDto.Username) || string.IsNullOrEmpty(registerUserDto.Email) || string.IsNullOrEmpty(registerUserDto.Password))
            //    {
            //        return Results.BadRequest("Please provide all required fields.");
            //    }

            //    var existingUser = await userManager.FindByEmailAsync(registerUserDto.Email);
            //    if (existingUser != null)
            //    {
            //        return Results.BadRequest("Email is already taken.");
            //    }

            //    var user = new User
            //    {
            //        UserName = registerUserDto.Username,
            //        Email = registerUserDto.Email
            //    };

            //    var result = await userManager.CreateAsync(user, registerUserDto.Password);
            //    if (result.Succeeded)
            //    {
            //        return Results.Ok("User registered successfully.");
            //    }
            //    else
            //    {
            //        return Results.BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            //    }
            //});

            users.MapPost("/logout", async (SignInManager<User> signInManager, [FromBody] object empty) =>
            {
                if (empty != null)
                {
                    await signInManager.SignOutAsync();
                    return TypedResults.Ok();
                }
                return Results.Unauthorized();

            });

            users.MapGet("/", async (UserDbContext idb) => await idb.Users.ToListAsync());
            users.MapGet("/{Id}", GetUserById);
            app.MapPost("/update-username", async (
                                            [FromServices] UserManager<User> userManager,
                                            [FromServices] IUserService userService,
                                            [FromBody] ChangeNameDto request,
                                            ClaimsPrincipal user) =>
            {
                var userId = userManager.GetUserId(user); // Получаем Id текущего пользователя
                if (userId != null)
                {
                    await userService.UpdateUserNameAsync(userId, request.newUsername);
                    return Results.Ok("Username updated successfully");
                }
                return Results.BadRequest("User not found");
            });

            static async Task<IResult> GetUserById(string id, UserDbContext udb)
            {
                return await udb.Users.FindAsync(id)
                    is User user
                        ? TypedResults.Ok(user)
                        : TypedResults.NotFound();
            }
        }
    }
}
