namespace ForumApi.Models.Dtos.Reply
{
    public class CreateReplyDto
    {
        public string Content { get; set; } = string.Empty;
        public int TopicId { get; set; }
        public int? ParentReplyId { get; set; } // Alt yanıt ise
    }
}
