using System.ComponentModel.DataAnnotations;

namespace ForumUi.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email alanı gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi girin.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        
        public string Password { get; set; } = string.Empty;
    }
}
