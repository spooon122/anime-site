using AnimeSite.Core.Models;
using System.Security.Claims;

namespace AnimeSite.Infrastructure.Authentication
{
    public interface IJwtTokenService
    {
        Task<string> GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string? token);
    }
}