using System.ComponentModel.DataAnnotations;

namespace anime_site.Users;

public record RegisterUserRequest(
    [Required] string Nickname,
    [Required] string Password,
    [Required] string Email);
