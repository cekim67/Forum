namespace ForumUi.Models
{
    public class ReplyDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public bool Liked { get; set; }
        public int? ParentReplyId { get; set; }     // Yanıta yanıt için
        public List<ReplyDto> ChildReplies { get; set; } = new List<ReplyDto>(); // Alt yanıtlar
    }
}