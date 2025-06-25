using AutoMapper;
using ForumApi.Context;
using ForumApi.Models.Dtos.Topic;
using ForumApi.Models.Dtos.User;
using ForumApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public UserService(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetMeAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<List<TopicResponseDto>> GetUserTopicsAsync(int userId)
        {
            var topics = await _context.Topics
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .Include(t => t.User)
                .ToListAsync();

            return _mapper.Map<List<TopicResponseDto>>(topics);
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.IsActive) return false;

            // Email ve username güncelle
            user.Username = dto.Username;
            user.Email = dto.Email;

            // Şifre değişecekse
            if (!string.IsNullOrEmpty(dto.CurrentPassword) && !string.IsNullOrEmpty(dto.NewPassword))
            {
                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                    throw new Exception("Mevcut şifre yanlış.");

                var newSalt = BCrypt.Net.BCrypt.GenerateSalt();
                var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword, newSalt);

                user.PasswordSalt = newSalt;
                user.PasswordHash = newHash;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
