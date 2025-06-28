using ForumApi.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumApi.Models.Entities
{
    [Table("Topics")]
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ViewCount { get; set; } = 0;
        


        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Reply> Replies { get; set; } = new List<Reply>();
        
    }
}