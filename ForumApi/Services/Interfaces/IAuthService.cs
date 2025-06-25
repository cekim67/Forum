using ForumApi.Models.Dtos.Auth;

namespace ForumApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
