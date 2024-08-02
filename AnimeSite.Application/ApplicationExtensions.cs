using AnimeSite.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AnimeSite.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<AnimeService>();
        
        return services;
    }
}