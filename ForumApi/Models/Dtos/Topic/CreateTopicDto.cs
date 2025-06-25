namespace ForumApi.Models.Dtos.Topic
{
    public class CreateTopicDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
