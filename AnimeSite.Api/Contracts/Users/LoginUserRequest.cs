using System.ComponentModel.DataAnnotations;

namespace anime_site.Users;

public class LoginUserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}