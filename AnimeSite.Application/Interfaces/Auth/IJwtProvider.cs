using AnimeSite.Core.Models;

namespace AnimeSite.Infrastructure;

public interface IJwtProvider
{
    string GenerateToken(User user);
}