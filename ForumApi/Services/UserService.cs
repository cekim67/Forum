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
                .Where(t => t.UserId == userId)
                .Include(t => t.User)
                .ToListAsync();
            return _mapper.Map<List<TopicResponseDto>>(topics);
        }
    }
}
