using anime_site.Users;
using AnimeSite.Application.Services;
using AnimeSite.DataAccess.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace anime_site.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("auth/registration", Register);

        app.MapPost("auth/login", Login);

        return app;
    }

    private static async Task<IResult> Register(
        RegisterUserRequest registerUserRequest, 
        UserService userService)
    {
        await userService.Register(registerUserRequest.Nickname, registerUserRequest.Email, registerUserRequest.Password);
        return Results.Ok();
    }
    private static async Task<IResult> Login(
        LoginUserRequest loginUserRequest,
        UserService userService)
    {
        var token = await userService.Login(loginUserRequest.Email, loginUserRequest.Password);
        return Results.Ok(token);
    }
}