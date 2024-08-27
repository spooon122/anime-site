

namespace anime_site.Dto
{
    public class RegisterRequestModel
    {
        
        public required string Username { get; set; }
        public string Email { get; set; }
        public required string Password { get; set; }
    }
}
