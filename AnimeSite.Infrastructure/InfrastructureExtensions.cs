using Microsoft.Extensions.DependencyInjection;

namespace AnimeSite.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHash, PasswordHash>();

        return services;
    }
}