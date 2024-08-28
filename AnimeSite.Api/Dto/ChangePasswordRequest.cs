namespace anime_site.Dto
{
    public class ChangePasswordRequest
    {
        public string Id { get; set; }
        public required string oldPassword {  get; set; }
        public required string newPassword { get; set; }
    }
}
