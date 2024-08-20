
using Microsoft.AspNetCore.Identity;

namespace AnimeSite.Core.Models;

public class User : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}