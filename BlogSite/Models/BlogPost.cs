using System.ComponentModel.DataAnnotations;

namespace BlogSite.Models;

public class BlogPost
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

