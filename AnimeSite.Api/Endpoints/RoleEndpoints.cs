using AnimeSite.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace anime_site.Endpoints
{
    public static class RoleEndpoints
    {
        public static void RolesEndpoints(this WebApplication app)
        {

            var roles = app.MapGroup("roles");

            roles.MapPost("/create-role", async (RoleManager<IdentityRole> roleManager, string roleName) =>
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    return Results.BadRequest($"Роль '{roleName}' уже существует");
                }

                var role = new IdentityRole(roleName);
                var result = await roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return Results.Ok($"Роль под названием '{roleName}' успешно создана.");
                }
                else
                {
                    return Results.BadRequest(result.Errors);
                }
            });

            app.MapPost("/assign-role", async ([FromServices] UserManager<User> userManager, string userEmail, string roleName) =>
            {
                var user = await userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return Results.BadRequest("Пользователь не найден :(");
                }

                var result = await userManager.AddToRoleAsync(user, roleName);

                if (result.Succeeded)
                {
                    return Results.Ok($"Роль '{roleName}' успешно присвоена.");
                }
                else
                {
                    return Results.BadRequest(result.Errors);
                }
            });
        }

    }
}
