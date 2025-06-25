using ForumApi.Models.Entities;

namespace ForumApi.Repositories.Interfaces
{
    public interface ITopicRepository
    {
        Task<List<Topic>> GetAllAsync();
        Task<Topic?> GetByIdAsync(int id);
        Task AddAsync(Topic topic);
        Task DeleteAsync(Topic topic);
        Task<Topic?> GetByIdWithUserAndLikesAsync(int id);
        
        Task SaveChangesAsync();
    }
}