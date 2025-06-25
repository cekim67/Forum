using ForumApi.Context;
using ForumApi.Models.Dtos.Like;
using ForumApi.Models.Entities;
using ForumApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services
{
    public class LikeService : ILikeService
    {
        private readonly ApiContext _context;

        public LikeService(ApiContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleLikeTopicAsync(int topicId, int userId)
        {
            var existing = await _context.Likes
                .FirstOrDefaultAsync(x => x.TopicId == topicId && x.UserId == userId);

            if (existing != null)
            {
                _context.Likes.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // Kaldırıldı
            }

            var like = new Like
            {
                UserId = userId,
                TopicId = topicId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true; // Eklendi
        }

        public async Task<bool> ToggleLikeReplyAsync(int replyId, int userId)
        {
            var existing = await _context.Likes
                .FirstOrDefaultAsync(x => x.ReplyId == replyId && x.UserId == userId);

            if (existing != null)
            {
                _context.Likes.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }

            var like = new Like
            {
                UserId = userId,
                ReplyId = replyId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTopicLikeCountAsync(int topicId)
        {
            return await _context.Likes.CountAsync(x => x.TopicId == topicId);
        }

        public async Task<int> GetReplyLikeCountAsync(int replyId)
        {
            return await _context.Likes.CountAsync(x => x.ReplyId == replyId);
        }


        public async Task<List<LikedTopicDto>> GetLikedTopicsAsync(int userId)
        {
            return await _context.Likes
                .Where(l => l.UserId == userId && l.TopicId != null)
                .Include(l => l.Topic!)
                .Select(l => new LikedTopicDto
                {
                    TopicId = l.TopicId.GetValueOrDefault(),
                    Title = l.Topic!.Title,
                    Content = l.Topic.Content,
                    CreatedAt = l.Topic.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<LikedReplyDto>> GetLikedRepliesAsync(int userId)
        {
            return await _context.Likes
                .Where(l => l.UserId == userId && l.ReplyId != null)
                .Include(l => l.Reply!)
                .Select(l => new LikedReplyDto
                {
                    ReplyId = l.ReplyId.GetValueOrDefault()  ,
                    TopicId = l.Reply!.TopicId,
                    Content = l.Reply.Content,
                    CreatedAt = l.Reply.CreatedAt
                })
                .ToListAsync();
        }

    }
}
