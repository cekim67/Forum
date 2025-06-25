using ForumApi.Context;
using ForumApi.Models.Dtos.Auth;
using ForumApi.Models.Entities;
using ForumApi.Services.Interfaces;
using ForumApi.Helpers;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ForumApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(ApiContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                return null;

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password, salt);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordSalt = salt,
                PasswordHash = hash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _jwtHelper.GenerateToken(user);
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (user == null) return null;

            var valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid) return null;
            if (!user.IsActive)
                throw new Exception("Hesabınız pasif. Giriş yapamazsınız.");

            return _jwtHelper.GenerateToken(user);
        }
    }
}
