using ForumApi.Models.Dtos.User;
using ForumApi.Models.Dtos.Topic;


namespace ForumApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetMeAsync(int userId);
        Task<List<TopicResponseDto>> GetUserTopicsAsync(int userId);
      

    }
}
