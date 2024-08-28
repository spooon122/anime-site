
using Microsoft.AspNetCore.Identity;

namespace AnimeSite.Core.Models;

public class User : IdentityUser
{
    public string? Description { get; set; }
}