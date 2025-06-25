using System.ComponentModel.DataAnnotations;

namespace ForumUi.Models
{
    public class CreateReplyViewModel
    {
        public int TopicId { get; set; }

        [Required(ErrorMessage = "Yanıt içeriği gereklidir.")]
        public string Content { get; set; } = string.Empty;

        public int? ParentReplyId { get; set; } // Yanıta yanıt için
    }
}