using ForumApi.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumApi.Models.Entities
{
    [Table("Replies")]
    public class Reply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        [Required]
        public int TopicId { get; set; }

        public int? ParentReplyId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int LikeCount { get; set; } = 0;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("TopicId")]
        public virtual Topic Topic { get; set; } = null!;

        [ForeignKey("ParentReplyId")]
        public virtual Reply? ParentReply { get; set; }

        // .NET 8.0 Collection Expression
        public virtual ICollection<Reply> ChildReplies { get; set; } = [];
        public virtual ICollection<Like> Likes { get; set; } = [];
    }
}