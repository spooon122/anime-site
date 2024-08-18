using System.ComponentModel.DataAnnotations;

namespace anime_site.Users;

public record RegisterUserRequest(
    [Required] string Username,
    [Required] string Email,
    [Required] string Password);
