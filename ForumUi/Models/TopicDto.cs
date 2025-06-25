namespace ForumUi.Models
{
    public class TopicDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public int LikeCount { get; set; }          // YENİ EKLENEN
        public bool Liked { get; set; }            // YENİ EKLENEN
    }
}