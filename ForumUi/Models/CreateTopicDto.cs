using System.ComponentModel.DataAnnotations;
namespace ForumUi.Models { 
public class CreateTopicDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}
}