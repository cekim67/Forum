using ForumApi.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumApi.Models.Entities
{
    [Table("Likes")]
    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int? TopicId { get; set; }

        public int? ReplyId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("TopicId")]
        public virtual Topic? Topic { get; set; }

        [ForeignKey("ReplyId")]
        public virtual Reply? Reply { get; set; }
    }
}
