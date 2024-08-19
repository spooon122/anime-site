using System.ComponentModel.DataAnnotations;

namespace anime_site.Dto;

public class RegisterUserDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public required string Password { get; set; }
}
