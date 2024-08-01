using anime_site.Endpoints;

namespace anime_site.Extansions;

public static class ApiExtensions
{
    public static void AddMapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapUsersEndpoints();
    }
}