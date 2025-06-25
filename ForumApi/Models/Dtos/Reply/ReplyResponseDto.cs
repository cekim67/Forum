namespace ForumApi.Models.Dtos.Reply
{
    public class ReplyResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<ReplyResponseDto>? ChildReplies { get; set; }
    }
}
