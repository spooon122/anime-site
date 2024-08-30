using AnimeSite.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

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

            app.MapPost("/upload", async (IFormFile file, [FromQuery] string bucketName) =>
            {
                var minio = new MinioClient()
                .WithCredentials("VwQNy2tuJ5Overvu1xw1", "90KHpJ7cLtN1vmcOZ2YOzw0qs4bSWR9Af5MzrTwv")
                .WithEndpoint("192.168.31.69:9000")
                .Build();

                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest("Файл не загружен.");
                }
                
                try
                {
                    string objectName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    await using var stream = file.OpenReadStream();
                    await minio.PutObjectAsync(
                        new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(objectName)
                            .WithStreamData(stream)
                            .WithObjectSize(file.Length)
                    );

                    return Results.Ok($"Файл успешно загружен как {objectName}.");
                }
                catch (MinioException ex)
                {
                    return Results.BadRequest($"Ошибка при загрузке файла: {ex.Message}");
                }
            }).DisableAntiforgery();
        }

    }
}
