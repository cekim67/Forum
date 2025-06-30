namespace ForumApi.Models.Dtos.Topic
{
    public class TopicResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
       
    }
}
