﻿using System.ComponentModel.DataAnnotations;

namespace ForumUi.Models { 
public class CreateReplyViewModel
{
    public int TopicId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;


    public int? ParentReplyId { get; set; }
    }
}