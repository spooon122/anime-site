
using Microsoft.AspNetCore.Identity;

namespace AnimeSite.Core.Models;

public class User : IdentityUser
{
    public Guid Id { get; set; }
    public string? userName { get; set; }
    public string? email { get; set; } = string.Empty;
    public string? password { get; set; }
}