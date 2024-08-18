using System.ComponentModel.DataAnnotations;

namespace anime_site.Users;

public record LoginUserRequest(
    [Required] string UserName,
    [Required] string Password,
    [Required] bool RememberMe);