
using ForumApi.Models.Dtos.Topic;


namespace ForumApi.Services.Interfaces
{
    public interface ITopicService
    {
        Task<List<TopicResponseDto>> GetAllAsync();
        Task<TopicResponseDto?> GetByIdAsync(int id);
        Task<TopicResponseDto> CreateAsync(CreateTopicDto dto, int userId);
        Task<bool> DeleteAsync(int id, int requestingUserId, bool isAdmin);
    }
}
