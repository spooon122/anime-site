﻿namespace anime_site.Dto
{
    public class LoginRequestDto
    {
        public bool RememberMe { get; set; }
        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
