using AnimeSite.Core.Models;
using AnimeSite.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;

namespace anime_site.Endpoints
{
    public static class UserEndpoints
    {
        public static void RegisterUserEndpoints(this WebApplication app) 
        {

            var users = app.MapGroup("users").RequireAuthorization();
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
            });
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
                var callback = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/users/forgot?userId={user.Id}&code={Uri.EscapeDataString(code)}";
                await emailSender.SendAsync(user.Email, "Send from animeq", callback);

                return Results.Ok("go!");
            });
            /// <summary>
            /// get all users
            /// </summary>
            users.MapPost("/", async (UserDbContext context) => await context.Users.ToListAsync());
        }
    }
}
