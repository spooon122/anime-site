using AnimeSite.Core.Abstractions;
using AnimeSite.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnimeSite.DataAccess;

public static class DataAccessExtensions
{
    public static IServiceCollection AddDataAccessExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(
            options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("AnimeSiteDbContext"));
            });
        services.AddDbContext<AnimeDbContext>(
            options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("AnimeSiteDbContext"));
            });
        
        services.AddScoped<IAnimeRepository, AnimeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}