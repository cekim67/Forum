using ForumApi.Models.Dtos.Like;

namespace ForumApi.Services.Interfaces
{
    public interface ILikeService
    {
        Task<bool> ToggleLikeTopicAsync(int topicId, int userId);
        Task<bool> ToggleLikeReplyAsync(int replyId, int userId);
        Task<int> GetTopicLikeCountAsync(int topicId);
        Task<int> GetReplyLikeCountAsync(int replyId);
        Task<List<LikedTopicDto>> GetLikedTopicsAsync(int userId);
        Task<List<LikedReplyDto>> GetLikedRepliesAsync(int userId);

    }
}
