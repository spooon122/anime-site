using anime_site.Dto;
using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;

namespace anime_site.Endpoints
{
    public static class UserEndpoints
    {
        public static void RegisterUserEndpoints(this WebApplication app) 
        {

            var users = app.MapGroup("users");
            /// <summary>
            /// method for loguot user
            /// </summary>
            users.MapPost("/logout", async (SignInManager<User> signInManager, [FromBody] object empty) =>
            {
                if (empty != null)
                {
                    await signInManager.SignOutAsync();
                    return Results.Ok();
                }
                return Results.Unauthorized();
            }).RequireAuthorization();

            /// <summary>
            /// method for forgot password with confirmed Email
            /// </summary>
            users.MapPost("/forgot", async (UserManager<User> userManager, ForgotPasswordRequest model, HttpContext httpContext, IEmailService emailSender) =>
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !(await userManager.IsEmailConfirmedAsync(user)))
                {
                    return Results.BadRequest("Email пользователя не подтвержден или пользователя не существует!");
                }

                var code = await userManager.GeneratePasswordResetTokenAsync(user);
                var callback = $"{httpContext.Request.Scheme}://localhost:5173/resetpassword?userId={user.Id}&resetCode={Uri.EscapeDataString(code)}";
                
                await emailSender.SendAsync(user.Email, "Send from animeq", callback);
                return Results.Ok("xd");
            });



            users.MapPost("/resetpassword", async (UserManager<User> userManager, ResetsPasswordRequest model, [FromQuery] string userId, [FromQuery] string resetCode) =>
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Results.NotFound("Пользователь не найден");
                }
                var result = await userManager.ResetPasswordAsync(user, Uri.UnescapeDataString(resetCode), model.NewPassword);
                
                if (result.Succeeded)
                {
                    return Results.Ok("Пароль успешно изменен!");
                }

                return Results.BadRequest(result.Errors);
            });

            /// <summary>
            /// get all users
            /// </summary>
            users.MapPost("/", [Authorize] async (UserDbContext context) => await context.Users.ToListAsync());
        }
    }
}
