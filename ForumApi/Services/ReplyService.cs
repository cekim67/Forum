using AutoMapper;
using ForumApi.Context;
using ForumApi.Models.Dtos.Reply;
using ForumApi.Models.Entities;
using ForumApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services
{
    public class ReplyService : IReplyService
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public ReplyService(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ReplyResponseDto> CreateAsync(CreateReplyDto dto, int userId)
        {
            var reply = _mapper.Map<Reply>(dto);
            reply.UserId = userId;
            _context.Replies.Add(reply);
            await _context.SaveChangesAsync();
            await _context.Entry(reply).Reference(r => r.User).LoadAsync();
            return _mapper.Map<ReplyResponseDto>(reply);
        }

        public async Task<List<ReplyResponseDto>> GetRepliesByTopicAsync(int topicId)
        {
            var replies = await _context.Replies
                .Where(r => r.TopicId == topicId && r.ParentReplyId == null && !r.IsDeleted)
                .Include(r => r.User)
                .Include(r => r.ChildReplies.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.User)
                .ToListAsync();

            return _mapper.Map<List<ReplyResponseDto>>(replies);
        }

        public async Task<bool> DeleteAsync(int replyId, int userId, bool isAdmin)
        {
            var reply = await _context.Replies.FindAsync(replyId);
            if (reply == null || reply.IsDeleted)
                return false;

            if (reply.UserId != userId && !isAdmin)
                return false;

            reply.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
