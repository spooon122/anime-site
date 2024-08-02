using System.ComponentModel.DataAnnotations;

namespace anime_site.Users;

public record LoginUserRequest(
    [Required] string Email,
    [Required] string Password);