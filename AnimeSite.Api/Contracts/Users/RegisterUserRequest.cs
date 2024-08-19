using System.ComponentModel.DataAnnotations;

namespace anime_site.Users;

public class RegisterUserRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
}
