using ForumApi.Models.Dtos.Reply;

namespace ForumApi.Services.Interfaces
{
    public interface IReplyService
    {
        Task<ReplyResponseDto> CreateAsync(CreateReplyDto dto, int userId);
        Task<List<ReplyResponseDto>> GetRepliesByTopicAsync(int topicId);
        Task<bool> DeleteAsync(int replyId, int userId, bool isAdmin);
    }
}
