using ForumApi.Models.Entities;
using ForumApi.Repositories.Interfaces;
using ForumApi.Context;
using Microsoft.EntityFrameworkCore; 
namespace ForumApi.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApiContext _context;
        public TopicRepository(ApiContext context)
        {
            _context = context;
        }
        public async Task<List<Topic>> GetAllAsync()
        {
            return await _context.Topics.Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        public async Task<Topic?> GetByIdAsync(int id)
        {
            return await _context.Topics.Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id );
        }
        public async Task AddAsync(Topic topic)
        {
            await _context.Topics.AddAsync(topic);
        }
        public async Task<Topic?> GetByIdWithUserAndLikesAsync(int id)
        {
            return await _context.Topics
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id );
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}