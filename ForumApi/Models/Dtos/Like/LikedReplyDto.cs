namespace ForumApi.Models.Dtos.Like
{
    public class LikedReplyDto
    {
        public int ReplyId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
