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

            var users = app.MapGroup("users").RequireAuthorization();

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
                    await userService.UpdateUserNameAsync(userId, request.newUsername!);
                    return Results.Ok("Имя пользователя успешно изменено!");
                }
                return Results.BadRequest("Пользователь не найден!");
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
