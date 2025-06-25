namespace ForumApi.Models.Dtos.User
{
    public class UpdateProfileDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CurrentPassword { get; set; } // şifre güncelleme istiyorsa kontrol için
        public string? NewPassword { get; set; }     // yeni şifre 
    }
}
