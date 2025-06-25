namespace ForumApi.Models.Dtos.Like
{
    public class LikedTopicDto
    {
        public int TopicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
